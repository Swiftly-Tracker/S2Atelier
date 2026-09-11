using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using S2Atelier.Automation;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = 16 * 1024);
builder.Services.AddSingleton<JobService>();
builder.Services.AddHostedService(s => s.GetRequiredService<JobService>());
var app = builder.Build();
var key = Environment.GetEnvironmentVariable("S2A_API_KEY");
if (string.IsNullOrEmpty(key) || key.Length < 32)
    throw new InvalidOperationException("S2A_API_KEY must contain at least 32 characters.");
var expected = SHA256.HashData(Encoding.UTF8.GetBytes("Bearer " + key));
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path == "/health") { await next(ctx); return; }
    var actual = SHA256.HashData(Encoding.UTF8.GetBytes(ctx.Request.Headers.Authorization.ToString()));
    if (!CryptographicOperations.FixedTimeEquals(expected, actual))
    { ctx.Response.StatusCode = 401; return; }
    await next(ctx);
});
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapPost("/jobs", (JobRequest request, JobService jobs) =>
{
    var error = request.Validate();
    if (error != null) return Results.BadRequest(new { error });
    var job = jobs.Enqueue(request);
    return job == null ? Results.StatusCode(429) : Results.Accepted($"/jobs/{job.Id}", job);
});
app.MapGet("/jobs", (JobService jobs) => jobs.List());
app.MapGet("/jobs/{id}", (string id, JobService jobs) => jobs.Get(id) is { } job ? Results.Ok(job) : Results.NotFound());
app.MapGet("/jobs/{id}/provenance", (string id, JobService jobs) =>
    jobs.Get(id) != null && File.Exists(jobs.PathFor(id, "provenance.json"))
        ? Results.File(jobs.PathFor(id, "provenance.json"), "application/json") : Results.NotFound());
app.MapGet("/jobs/{id}/log", (string id, JobService jobs) =>
    jobs.Get(id) != null && File.Exists(jobs.PathFor(id, "pipeline.log"))
        ? Results.File(jobs.PathFor(id, "pipeline.log"), "text/plain") : Results.NotFound());
app.MapGet("/jobs/{id}/artifacts/{index:int}", (string id, int index, JobService jobs) =>
{
    var job = jobs.Get(id);
    if (job?.State != "succeeded" || index < 0 || index >= job.Artifacts.Count) return Results.NotFound();
    var path = jobs.PathFor(id, job.Artifacts[index]);
    return Results.File(path, "application/octet-stream", Path.GetFileName(path), enableRangeProcessing: true);
});
app.Run();

namespace S2Atelier.Automation
{
    public sealed record JobRequest(string DumpsCommit, string BinaryRegex, string Platform,
        bool ImportSchema = true, uint? DepotId = null, string? ManifestId = null, uint? AppId = null)
    {
        // Retain legacy fields only to read historical job.json files after an upgrade.
        public string? Validate()
        {
            if (DepotId != null || ManifestId != null || AppId != null)
                return "depotId, manifestId and appId are read from dumpsCommit; omit these fields.";
            if (string.IsNullOrEmpty(DumpsCommit) || !Regex.IsMatch(DumpsCommit, "\\A[0-9a-fA-F]{40}\\z"))
                return "dumpsCommit must be a full 40-character Git commit SHA.";
            if (Platform is not ("windows" or "linux")) return "platform must be windows or linux.";
            if (string.IsNullOrWhiteSpace(BinaryRegex) || BinaryRegex.Length > 512 || BinaryRegex.Any(char.IsControl))
                return "binaryRegex must contain 1–512 characters without control characters.";
            try { _ = new Regex(BinaryRegex, RegexOptions.NonBacktracking, TimeSpan.FromSeconds(1)); }
            catch (Exception e) when (e is ArgumentException or NotSupportedException) { return "Invalid or unsupported regex: " + e.Message; }
            return null;
        }
    }
    public sealed record Job(string Id, JobRequest Request, string State, DateTimeOffset CreatedAt,
        DateTimeOffset? FinishedAt = null, string? Error = null, List<string>? Files = null)
    {
        public List<string> Artifacts => Files ?? [];
    }
    public sealed class JobService : BackgroundService
    {
        private readonly object gate = new();
        private readonly Dictionary<string, Job> jobs = [];
        private readonly SemaphoreSlim signal = new(0);
        private readonly string root = Environment.GetEnvironmentVariable("S2A_ROOT") ?? "/opt/s2atelier";
        private readonly ILogger<JobService> logger;
        private readonly FileStream instanceLock;
        public JobService(ILogger<JobService> logger)
        {
            this.logger = logger;
            Directory.CreateDirectory(Path.Combine(root, "jobs"));
            instanceLock = new FileStream(Path.Combine(root, "service.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            foreach (var file in Directory.EnumerateFiles(Path.Combine(root, "jobs"), "job.json", SearchOption.AllDirectories))
            {
                var job = JsonSerializer.Deserialize<Job>(File.ReadAllText(file))!;
                if (job.State == "running") job = job with { State = "failed", Error = "Service interrupted; submit a new job.", FinishedAt = DateTimeOffset.UtcNow };
                if (job.State == "queued" && job.Request.Validate() is { } error)
                    job = job with { State = "failed", Error = "Request format changed; resubmit with dumpsCommit and platform. " + error, FinishedAt = DateTimeOffset.UtcNow };
                Save(job);
                if (job.State == "queued") signal.Release();
            }
        }
        public string PathFor(string id, string file) => Path.Combine(root, "jobs", id, file);
        public Job? Get(string id) { lock (gate) return jobs.GetValueOrDefault(id); }
        public Job[] List() { lock (gate) return jobs.Values.OrderByDescending(j => j.CreatedAt).Take(100).ToArray(); }
        public Job? Enqueue(JobRequest request)
        {
            lock (gate)
            {
                if (jobs.Values.Count(j => j.State is "queued" or "running") >= 32) return null;
                var job = new Job(Guid.NewGuid().ToString("N"), request, "queued", DateTimeOffset.UtcNow);
                Directory.CreateDirectory(PathFor(job.Id, ""));
                Save(job);
                signal.Release();
                return job;
            }
        }
        private void Save(Job job)
        {
            lock (gate)
            {
                File.WriteAllText(PathFor(job.Id, "job.json.tmp"), JsonSerializer.Serialize(job));
                File.Move(PathFor(job.Id, "job.json.tmp"), PathFor(job.Id, "job.json"), true);
                jobs[job.Id] = job;
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try { await signal.WaitAsync(stoppingToken); } catch (OperationCanceledException) { break; }
                Job? job;
                lock (gate) job = jobs.Values.Where(j => j.State == "queued").OrderBy(j => j.CreatedAt).FirstOrDefault();
                if (job == null) continue;
                Save(job = job with { State = "running" });
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                timeout.CancelAfter(TimeSpan.FromHours(6));
                using var process = new Process { StartInfo = new ProcessStartInfo("python3") { UseShellExecute = false } };
                process.StartInfo.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "pipeline.py"));
                process.StartInfo.ArgumentList.Add(root);
                process.StartInfo.ArgumentList.Add(PathFor(job.Id, "job.json"));
                try
                {
                    process.Start();
                    await process.WaitForExitAsync(timeout.Token);
                    if (process.ExitCode != 0) throw new Exception($"Pipeline exited {process.ExitCode}; inspect /jobs/{job.Id}/log.");
                    var files = Directory.GetFiles(PathFor(job.Id, "artifacts"), "*.i64", SearchOption.AllDirectories)
                        .Where(p => new FileInfo(p).Length > 0).Select(p => Path.GetRelativePath(PathFor(job.Id, ""), p)).ToList();
                    if (files.Count == 0) throw new Exception("Pipeline produced no i64 artifacts.");
                    Save(job with { State = "succeeded", FinishedAt = DateTimeOffset.UtcNow, Files = files });
                }
                catch (Exception e)
                {
                    try { if (!process.HasExited) process.Kill(entireProcessTree: true); } catch (InvalidOperationException) { }
                    // A killed docker client does not stop its container.
                    foreach (var platform in new[] { "windows", "linux" })
                    {
                        try
                        {
                            using var cleanup = Process.Start(new ProcessStartInfo("docker") { ArgumentList = { "rm", "-f", $"s2a-{job.Id}-{platform}" }, RedirectStandardOutput = true, RedirectStandardError = true });
                            if (cleanup != null) await cleanup.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(15));
                        }
                        catch (Exception cleanupError) { logger.LogWarning(cleanupError, "Container cleanup failed"); }
                    }
                    Save(job with { State = "failed", FinishedAt = DateTimeOffset.UtcNow, Error = e is OperationCanceledException ? "Job timed out or service stopped." : e.Message });
                    logger.LogError(e, "Job {Id} failed", job.Id);
                }
            }
        }
        public override void Dispose() { base.Dispose(); instanceLock.Dispose(); signal.Dispose(); }
    }
}
