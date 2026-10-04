namespace GenerativeAI_dotnet;

public record ActionItem(
    string Task,
    string? Assignee,
    string? DueDate,
    string Priority);
