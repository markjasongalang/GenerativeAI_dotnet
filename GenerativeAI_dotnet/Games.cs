namespace GenerativeAI_dotnet;

public class Games
{
    public string? Query { get; init; }
    public int TotalResults { get; init; }
    public int Limit { get; init; }
    public int Offset { get; init; }
    public IList<GameSearchResult>? Results { get; init; }
}

public class GameSearchResult
{
    public long Id { get; init; }
    public decimal Year { get; init; }
    public string? Name { get; init; }
    public string? Genre { get; init; }
    public string? ShortDescription { get; init; }
}
