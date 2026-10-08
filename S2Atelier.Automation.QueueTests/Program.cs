using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using S2Atelier.Automation;

var root = Path.Combine(Path.GetTempPath(), "s2atelier-queue-" + Guid.NewGuid().ToString("N"));
Environment.SetEnvironmentVariable("S2A_ROOT", root);
var request = new JobRequest(new string('a', 40), null, "all");
void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
try
{
    foreach (var state in new[] { "succeeded", "failed", "queued" })
    {
        var dir = Path.Combine(root, "jobs", state);
        Directory.CreateDirectory(dir);
        var job = new Job(state, request, state, DateTimeOffset.UtcNow.AddHours(-1));
        File.WriteAllText(Path.Combine(dir, "job.json"), JsonSerializer.Serialize(job));
    }
    using var service = new JobService(NullLogger<JobService>.Instance);
    var first = service.Enqueue(request)!;
    var second = service.Enqueue(request)!;
    Check(first.Id != second.Id && first.State == "queued" && second.State == "queued", "Repeated submissions must create distinct queued jobs");
    foreach (var state in new[] { "succeeded", "failed", "queued" })
        Check(service.Get(state)!.State == state, "Historical job was changed: " + state);
    Check(service.List().Length == 5, "Historical or new jobs missing");
    var saved = JsonSerializer.Deserialize<Job>(File.ReadAllText(service.PathFor(second.Id, "job.json")))!;
    Check(saved.Id == second.Id, "New submission was not persisted");
    for (var i = 0; i < 29; i++) Check(service.Enqueue(request) != null, "Queue filled too early");
    Check(service.Enqueue(request) == null, "Queue capacity must remain 32");
    Console.WriteLine("PASS: repeat submissions create distinct durable jobs; history preserved; capacity 32 enforced.");
}
finally { Directory.Delete(root, true); }
