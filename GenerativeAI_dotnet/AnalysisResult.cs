namespace GenerativeAI_dotnet;

public record AnalysisResult(
    string Product,
    string OverallSentiment,
    double AverageRating,
    int ReviewCount,
    List<string> Strengths,
    List<string> Weakness,
    List<string> RecommendActions,
    bool NeedsUrgentAttention);
