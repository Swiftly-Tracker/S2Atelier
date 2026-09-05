using System.Text.Json;

namespace S2Atelier.Ida.Worker;

internal static class WorkerProtocol
{
    internal static string Write(WireMessage message) => JsonSerializer.Serialize(message, WireJsonContext.Default.WireMessage);

    internal static WireMessage? Read(string line)
    {
        try
        {
            return JsonSerializer.Deserialize(line, WireJsonContext.Default.WireMessage);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
