using System.Text.Json;

namespace GenerativeAI_dotnet;

public static class JsonDefaults
{
    public static readonly JsonSerializerOptions SnakeCaseLower = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
}
