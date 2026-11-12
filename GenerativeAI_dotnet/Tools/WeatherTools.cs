using System.ComponentModel;
using System.Text.Json;
using GenerativeAI_dotnet.Configurations;
using Microsoft.Extensions.Configuration;

namespace GenerativeAI_dotnet.Tools;

public class WeatherTools
{
    private readonly HttpClient _httpClient;
    private readonly WeatherApiSettings _weatherApiSettings;

    public WeatherTools(HttpClient httpClient, IConfiguration configuration, string environment)
    {
        _httpClient = httpClient;

        _weatherApiSettings = configuration
            .GetSection(nameof(WeatherApiSettings))
            .Get<WeatherApiSettings>()
            ?? throw new InvalidOperationException($"{nameof(WeatherApiSettings)} section is missing in appsettings.{environment}.json");
    }

    /// <summary>
    /// Helper function/tool for AI to access the real-time weather data from <see href="https://www.weatherapi.com/">WeatherAPI.com</see>.
    /// </summary>
    /// <param name="location">The location for which to get weather information.</param>
    /// <returns>The summary of weather information for a location.</returns>
    [Description("Get the current weather for a location")]
    public async Task<string> GetWeatherAsync(string location)
    {
        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync(
                $"{_weatherApiSettings.BaseUrl}/current.json?q={location}&key={_weatherApiSettings.ApiKey}");

            var currentWeather = JsonSerializer.Deserialize<CurrentWeather>(
                await response.Content.ReadAsStringAsync(),
                JsonDefaults.SnakeCaseLower);

            return
                $""" 
                {currentWeather.Location.Name}, {currentWeather.Location.Country} has {currentWeather.Current.Condition.Text} condition as of {currentWeather.Location.Localtime}.
                The temperature is {currentWeather.Current.TempC} celsius.
                The actual wind is {currentWeather.Current.WindMph} miles per hour.
                It feels like {currentWeather.Current.FeelslikeC} celsius.
                It's currently {(currentWeather.Current.IsDay == 1 ? "day" : "night")}.
                There's a {currentWeather.Current.ChanceOfRain}% chance of rain.
                """;
        }
        catch (Exception)
        {
            return "The weather information cannot be retrieved at this time. Please try again later.";
        }
    }
}
