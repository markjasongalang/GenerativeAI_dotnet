namespace GenerativeAI_dotnet;

public record SentimentAnalysis(
    string ResponseText,
    Sentiment ReviewSentiment,
    double ConfidenceScore,
    string[] KeyPhrases);
