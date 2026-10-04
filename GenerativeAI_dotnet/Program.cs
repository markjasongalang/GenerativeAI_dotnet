using System.Text;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace GenerativeAI_dotnet;

public class Program
{
    public static async Task Main(string[] args)
    {
        IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "gemma3:1b");


    }

    /// <summary>
    /// AI analyzes the sentiment of customer reviews.
    /// </summary>
    /// <param name="chatClient">The client abstraction for interacting with AI services that provide chat capabilities.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SentimentAnalysis(IChatClient chatClient)
    {
        var prompt = new StringBuilder();
        prompt.AppendLine("Analyze the sentiment of each review. Output: Review number, Sentiment (Positive/Negative/Neutral), Key reason.");
        prompt.AppendLine();
        prompt.AppendLine("Review 1: I bought this product and it's amazing. I love it!");
        prompt.AppendLine("Review 2: This product is terrible. I hate it.");
        prompt.AppendLine("Review 3: I'm not sure about this product. It's okay.");

        ChatResponse response = await chatClient.GetResponseAsync(prompt.ToString());
        Console.WriteLine(response.Text);
    }

    /// <summary>
    /// The AI's behavior is controlled by the system message (pre-prompt) to be a "Senior .NET Developer".
    /// </summary>
    /// <param name="chatClient">The client abstraction for interacting with AI services that provide chat capabilities.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task ExpertDotnetDeveloper(IChatClient chatClient)
    {
        chatClient = ChatClientBuilderChatClientExtensions.AsBuilder(chatClient)
            .ConfigureOptions(options => options.Temperature ??= 0.4f)
            .Build();

        var history = new List<ChatMessage>()
        {
            // System message - influences every response (main control mechanism; pre-prompt)
            new ChatMessage(ChatRole.System,
                "You are a senior .NET developer. Answer questions about C# and .NET. " +
                "Keep responses concise (maximum 3 sentences). " +
                "If asked about other topics, politely redirect to .NET topics. " +
                "Rules: " +
                "- Keep answers concise and practical. " +
                //"- Only answer questions related to .NET topics, simply respond that you stricty cater to .NET topics only. " +
                //"- If asked about non-.NET topics, simply respond that you stricty cater to .NET topics only. " +
                //"- Don't answer sneakily in your responses if the question or topic is not .NET, simply respond that you stricty cater to .NET topics only. " +
                //"- Never answer topics or questions unrelated to .NET, simply respond that you stricty cater to .NET topics only. " +
                "- Never generate code on questions or topics NOT RELATED  .NET, simply respond that you stricty cater to .NET topics only. " +
                "- If the question or topic is not related to .NET, simply respond EXACTLY (don't add anything): \"I only cater to .NET topics.\")")
        };

        // Since AI has no memory, we have to maintain a conversation history
        while (true)
        {
            Console.Write("Q: ");
            history.Add(new(ChatRole.User, $"{Console.ReadLine()}"));

            ChatResponse response = await chatClient.GetResponseAsync(history);
            Console.WriteLine(response.Text);

            history.AddMessages(response); // Extracts messages from the ChatResponse and adds them to the history
        }
    }

    /// <summary>
    /// A working chat application.
    /// </summary>
    /// <param name="chatClient">The client abstraction for interacting with AI services that provide chat capabilities.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SimpleChatApp(IChatClient chatClient)
    {
        var conversation = new List<ChatMessage>()
        {
            new ChatMessage(
                ChatRole.System,
                "You are a helpful assistant. Be concise but friendly.")
        };

        Console.WriteLine("Chat started. Type 'quit' to exit.\n");

        while (true)
        {
            Console.Write("You: ");
            string? userInput = Console.ReadLine();

            if (string.IsNullOrEmpty(userInput) || userInput.ToLower() == "quit")
            {
                break;
            }

            // Add user message to history
            conversation.Add(new ChatMessage(ChatRole.User, userInput));

            // Add AI response to history
            ChatResponse response = await chatClient.GetResponseAsync(conversation);
            conversation.AddMessages(response);

            Console.WriteLine($"AI: {response.Text}\n");
        }
    }

    /// <summary>
    /// Streaming lets you display tokens as they arrive, creating a more responsive experience, like what you see in ChatGPT or Copilot.
    /// </summary>
    /// <param name="chatClient">The client abstraction for interacting with AI services that provide chat capabilities.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task BasicStreamingPattern(IChatClient chatClient)
    {
        var chatHistory = new List<ChatMessage>();

        while (true)
        {
            Console.Write("Q: ");
            chatHistory.Add(new ChatMessage(ChatRole.User, Console.ReadLine()));

            var updates = new List<ChatResponseUpdate>();
            await foreach (ChatResponseUpdate update in chatClient.GetStreamingResponseAsync(chatHistory))
            {
                Console.Write(update.Text);
                updates.Add(update);
            }
            Console.WriteLine();

            // Add the streamed response to history
            chatHistory.AddMessages(updates);
        }
    }

    /// <summary>
    /// Microsoft.Extensions.AI (MEAI) provides structure output support that return strongly-typed objects instead of plain text.
    /// </summary>
    /// <param name="chatClient">The client abstraction for interacting with AI services that provide chat capabilities.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task BasicStructuredOutput(IChatClient chatClient)
    {
        var reviews = new string[]
        {
            "Best purchase ever!",
            "Returned it immediately.",
            "Hello",
            "It works as advertised.",
            "The packaging was damaged but otherwise okay."
        };

        foreach (string review in reviews)
        {
            // Map to Sentiment enum
            ChatResponse<Sentiment> response = await chatClient.GetResponseAsync<Sentiment>(
                $"What's the sentiment of this review? {review}");

            Console.WriteLine($"Review: {review} | Sentiment: {response.Result}");
        }
    }

    /// <summary>
    /// Chat response is mapped to a record type (for complex responses).
    /// </summary>
    /// <param name="chatClient">The client abstraction for interactiing with AI services that provide chat capabilities.</param>
    /// <returns>A task that represents an asynchronous operation.</returns>
    public static async Task ComplexStructuredOutputWithRecords(IChatClient chatClient)
    {
        var review = "This product exceeded my expectations in every way!";
        ChatResponse<SentimentAnalysis> response = await chatClient.GetResponseAsync<SentimentAnalysis>(
            $"Analyze this review: {review}");

        Console.WriteLine($"Text: {response.Result.ResponseText}");
        Console.WriteLine($"Sentiment: {response.Result.ReviewSentiment}");
        Console.WriteLine($"Confidence: {response.Result.ConfidenceScore}");
        Console.WriteLine($"Key phrases: {string.Join(", ", response.Result.KeyPhrases)}");
    }

    /// <summary>
    /// AI extracts contact information from the sample text and the response is mapped to <see cref="ContactInfo"/>.
    /// </summary>
    /// <param name="chatClient">The client abstraction for interactiing with AI services that provide chat capabilities.</param>
    /// <returns>A task that represents an asynchronous operation.</returns>
    public static async Task ExtractContactInformation(IChatClient chatClient)
    {
        var text = @"
            Hi, I'm Sarah Johnson from Contoso Ltd. 
            You can reach me at sarah.johnson@contoso.com 
            or call my office at 555-0123.
        ";

        ChatResponse<ContactInfo> response = await chatClient.GetResponseAsync<ContactInfo>(
            $"Extract contact information from this text: {text}");

        ContactInfo contact = response.Result;
        Console.WriteLine($"Name: {contact.Name}");
        Console.WriteLine($"Email: {contact.Email}");
        Console.WriteLine($"Phone: {contact.Phone}");
        Console.WriteLine($"Company: {contact.Company}");
    }

    /// <summary>
    /// AI analyzes meeting notes and maps the items to a list of type <see cref="ActionItem"/>.
    /// </summary>
    /// <param name="chatClient">The client abstraction for interactiing with AI services that provide chat capabilities.</param>
    /// <returns>A task that represents an asynchronous operation.</returns>
    public static async Task ListAndCollectionsStructuredOutput(IChatClient chatClient)
    {
        var meetingNotes = @"
            Discussed Q1 roadmap. John will prepare the budget by Friday (high priority).
            Sarah needs to review the marketing plan next week (medium priority).
            Team should update documentation ongoing (low priority).
        ";

        ChatResponse<List<ActionItem>> response = await chatClient.GetResponseAsync<List<ActionItem>>(
            $"Extract all action items from these meeting notes: {meetingNotes}");

        foreach (ActionItem item in response.Result)
        {
            Console.WriteLine($"- {item.Task} | {item.Assignee} | {item.DueDate} | {item.Priority}");
        }
    }

    /// <summary>
    /// AI analyzes a customer views for a coffee maker (user query), then streams its thinking, and finally maps the 
    /// final analysis to <see cref="AnalysisResult"/>.
    /// </summary>
    /// <param name="chatClient">The client abstraction for interactiing with AI services that provide chat capabilities.</param>
    /// <returns>A task that represents an asynchronous operation.</returns>
    public static async Task CombiningStreamingAndStructuredOutput(IChatClient chatClient)
    {
        var userQuery =
            """
            Analyze these customer reviews for the "AeroBrew" coffee maker and tell me
            what customers think overall:

            1. "Brews fast and the coffee tastes great, but the lid feels flimsy." (4 stars)
            2. "Stopped working after 2 months. Support was slow to respond." (1 star)
            3. "Love the sleek design and the timer feature. Worth the price!" (5 stars)
            4. "Water reservoir is too small for a family. Otherwise decent." (3 stars)
            5. "Easy to clean, quiet, and makes a perfect cup every time." (5 stars)
            """;

        Console.WriteLine("AI is analyzing...");
        var fullText = "";
        await foreach (ChatResponseUpdate update in chatClient.GetStreamingResponseAsync(userQuery))
        {
            Console.Write(update.Text);
            fullText += update.Text;
        }

        ChatResponse<AnalysisResult> structured = await chatClient.GetResponseAsync<AnalysisResult>(
            $"Extract key data from: {fullText}");

        AnalysisResult analysisResult = structured.Result;
        Console.WriteLine($"Product: {analysisResult.Product}");
        Console.WriteLine($"Overall Sentiment: {analysisResult.OverallSentiment}");
        Console.WriteLine($"Average Rating: {analysisResult.AverageRating}");
        Console.WriteLine($"Review Count: {analysisResult.ReviewCount}");
        Console.WriteLine($"Strengths: {string.Join(", ", analysisResult.Strengths)}");
        Console.WriteLine($"Recommended Actions: {string.Join(", ", analysisResult.RecommendActions)}");
        Console.WriteLine($"Needs Urgent Attention: {analysisResult.NeedsUrgentAttention}");
    }
}
