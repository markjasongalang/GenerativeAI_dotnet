namespace GenerativeAI_dotnet;

public class DeviceSearch
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
    public IList<Devices>? Devices { get; set; }
}

public class Devices
{
    public long Id { get; set; }
    public string? MatchCertainty { get; set; }
    public string? MatchType { get; set; }
    public string? Name { get; set; }
    public string? ImageB64 { get; set; }
    public string? ManufacturerName { get; set; }
    public string? DeviceType { get; set; }
    public string? Description { get; set; }
    public string? Colors { get; set; }
    public string? Storage { get; set; }
    public string? ScreenResolution { get; set; }
    public string? Weight { get; set; }
    public string? Thickness { get; set; }
    public string? ReleaseDate { get; set; }
    public string? Camera { get; set; }
    public string? BatteryCapacity { get; set; }
    public string? Hardware { get; set; }
    public string? ModelNumbers { get; set; }
}
