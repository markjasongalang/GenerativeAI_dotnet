using System.ComponentModel;
using System.Text;
using System.Text.Json;
using GenerativeAI_dotnet.Configurations;
using Microsoft.Extensions.Configuration;

namespace GenerativeAI_dotnet.Tools;

public class SearchGamesTools
{
    private readonly HttpClient _httpClient;
    private readonly GamesApiSettings _gamesApiSettings;

    public SearchGamesTools(HttpClient httpClient, IConfiguration configuration, string environment)
    {
        _httpClient = httpClient;

        _gamesApiSettings = configuration
            .GetSection(nameof(GamesApiSettings))
            .Get<GamesApiSettings>()
            ?? throw new InvalidOperationException($"{nameof(GamesApiSettings)} section is missing in appsettings.{environment}.json");
    }

    /// <summary>
    /// Helper functions for AI to search games.
    /// </summary>
    /// <param name="query">The search term for the game.</param>
    /// <returns>The list of games that matched the search.</returns>
    [Description("Search games based on query")]
    public async Task<string> SearchGamesAsync([Description("The search query for games")] string query)
    {
        try
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_gamesApiSettings.BaseUrl}/games?query={query}&limit=5"),
                Headers =
                {
                    { "Accept", "application/json" },
                    { "x-api-key", _gamesApiSettings.ApiKey }
                }
            };

            using HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

            var games = JsonSerializer.Deserialize<Games>(
                await response.Content.ReadAsStringAsync(),
                JsonDefaults.SnakeCaseLower);

            var resultBuilder = new StringBuilder();

            foreach (var game in games?.Results ?? Array.Empty<GameSearchResult>())
            {
                resultBuilder.AppendLine($"Game: {game.Name} ({Convert.ToInt32(game.Year)})");
                resultBuilder.AppendLine($"Genre: {game.Genre}");
                resultBuilder.AppendLine($"Description: {game.ShortDescription}");
                resultBuilder.AppendLine();
            }

            return resultBuilder.ToString();
        }
        catch (Exception)
        {
            return "The game search cannot be performed at this time. Please try again later.";
        }
    }
}
