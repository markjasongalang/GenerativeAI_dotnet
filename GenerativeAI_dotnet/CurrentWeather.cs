namespace GenerativeAI_dotnet;

/// <summary>
/// The current weather response (schema) from <see href="https://www.weatherapi.com/">WeatherAPI.com</see>.
/// </summary>
public record CurrentWeather(Location? Location = null, Current? Current = null);

/// <remarks>
/// You can view each field's data type and description below:
/// <list type="bullet">
/// <item><see href="https://www.weatherapi.com/docs/realtime-api/"/></item>
/// <item><see href="https://www.weatherapi.com/docs/forecast-api/"/></item>
/// </list>
/// </remarks>
public class Location
{
    public string? Name { get; init; }
    public string? Region { get; init; }
    public string? Country { get; init; }
    public decimal? Lat { get; init; }
    public decimal? Lon { get; init; }
    public string? TzId { get; init; }
    public int? LocaltimeEpoch { get; init; }

    /// <remarks>
    /// <para>
    /// TODO: Change this to <see cref="DateTime"/> data type.
    /// </para>
    /// <para>
    /// Refer to <see href="https://stackoverflow.com/questions/55502354/converting-string-to-datetime-while-deserializing"/>
    /// </para>
    /// </remarks>
    public string? Localtime { get; init; }
}

public class Current
{
    public int? LastUpdatedEpoch { get; init; }
    public string? LastUpdated { get; init; }
    public decimal? TempC { get; init; }
    public decimal? TempF { get; init; }
    public int? IsDay { get; init; } // 1 = Yes, 0 = No (whether to show day condition or night icon)
    public Condition? Condition { get; init; }
    public decimal? WindMph { get; init; }
    public decimal? WindKph { get; init; }
    public decimal? WindDegree { get; init; }
    public string? WindDir { get; init; }
    public decimal? PressureMb { get; init; }
    public decimal? PressureIn { get; init; }
    public decimal? PrecipMm { get; init; }
    public decimal? PrecipIn { get; init; }
    public int? Humidity { get; init; } // Percentage
    public int? Cloud { get; init; } // Cloud cover as percentage
    public decimal? FeelslikeC { get; init; }
    public decimal? FeelslikeF { get; init; }
    public decimal? WindchillC { get; init; }
    public decimal? WindchillF { get; init; }
    public decimal? HeatindexC { get; init; }
    public decimal? HeatindexF { get; init; }
    public decimal? DewpointC { get; init; }
    public decimal? DewpointF { get; init; }
    public decimal? VisKm { get; init; }
    public decimal? VisMiles { get; init; }
    public decimal? Uv { get; init; }
    public decimal? GusMph { get; init; }
    public decimal? GusKph { get; init; }
    public int? WillItRain { get; init; }
    public decimal? ChanceOfRain { get; init; } // Percentage
    public int? WillItSnow { get; init; } // Percentage
    public decimal? ChanceOfSnow { get; init; }
    public decimal? WetbulbC { get; init; }
    public decimal? WetbulbF { get; init; }
    public decimal? ShortRoad { get; init; }
    public decimal? DiffRad { get; init; }
    public decimal? Dni { get; init; }
    public decimal? Gti { get; init; }
}

public class Condition
{
    public string? Text { get; set; }
    public string? Icon { get; set; }
    public int? Code { get; set; }
}
