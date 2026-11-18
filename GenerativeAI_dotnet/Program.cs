using System.ComponentModel;
using System.Text;
using GenerativeAI_dotnet.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OllamaSharp;

namespace GenerativeAI_dotnet;

public class Program
{
    /// <summary>
    /// According to the <see href="https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines">guidelines</see>,
    /// the recommended approach is to reuse <see cref="HttpClient"/> instances during the application lifecycle.
    /// </summary>
    private static HttpClient _httpClient = new();

    public static async Task Main(string[] args)
    {
        string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT", EnvironmentVariableTarget.User) 
            ?? "Production";

        //Console.WriteLine(AppContext.BaseDirectory);

        IConfigurationRoot? configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();

#if DEBUG
        Console.Clear();
#endif

        await MultiplePracticalFunctions();
    }

    /// AI analyzes the sentiment of customer reviews.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SentimentAnalysis()
    {
        IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "gemma3:1b");

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
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task ExpertDotnetDeveloper()
    {
        IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "gemma3:1b");

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
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task SimpleChatApp()
    {
        IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "gemma3:1b");

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
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task BasicStreamingPattern()
    {
        IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "gemma3:1b");

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
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task BasicStructuredOutput()
    {
        IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "gemma3:1b");

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
    /// <returns>A task that represents an asynchronous operation.</returns>
    public static async Task ComplexStructuredOutputWithRecords()
    {
        IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "gemma3:1b");

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
    /// <returns>A task that represents an asynchronous operation.</returns>
    public static async Task ExtractContactInformation()
    {
        IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "gemma3:1b");

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
    /// <returns>A task that represents an asynchronous operation.</returns>
    public static async Task ListAndCollectionsStructuredOutput()
    {
        IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "gemma3:1b");

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
    /// <returns>A task that represents an asynchronous operation.</returns>
    public static async Task CombiningStreamingAndStructuredOutput()
    {
        IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "gemma3:1b");

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

    /// <summary>
    /// AI models utilizes <see cref="GetWeather(string)"/> function call in order to formulate a response
    /// in getting the current weather which came from a prompt.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// Refer to <see href="https://learn.microsoft.com/en-us/dotnet/ai/ichatclient#tool-calling"/>
    /// </remarks>
    public static async Task FirstFunctionCall()
    {
        IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "granite4:350m");

        chatClient = ChatClientBuilderChatClientExtensions
            .AsBuilder(chatClient)
            .UseFunctionInvocation()
            .Build();

        // Define available functions
        var chatOptions = new ChatOptions
        {
            Tools = [AIFunctionFactory.Create(GetWeather)] // Wraps method as AI-callable function
        };

        IAsyncEnumerable<ChatResponseUpdate> response = chatClient.GetStreamingResponseAsync(
            "Should I bring an umbrella to Seattle today?",
            chatOptions);

        await foreach (ChatResponseUpdate update in response)
        {
            Console.Write(update);
        }

        Console.WriteLine();
    }

    #region First function call helper

    /// <summary>
    /// Gives AI the ability to check the weather.
    /// </summary>
    /// <param name="location">The location of the weather to be determined.</param>
    /// <returns>The temperature and conditions of the specified location.</returns>
    /// <remarks>
    /// The <see cref="DescriptionAttribute"/> tells AI what this function does.
    /// </remarks>
    [Description("Get the current weather for a location")]
    public static string GetWeather(string location)
    {
        int temperature = Random.Shared.Next(15, 30);
        string conditions = Random.Shared.Next(0, 2) == 0 ? "sunny" : "cloudy";
        return $"{location}: {temperature}°C and {conditions}";
    }

    #endregion

    /// <summary>
    /// AI learns the parameter types and its purposes.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task FunctionCallWithParams()
    {
        IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "granite4:350m");

        chatClient = ChatClientBuilderChatClientExtensions
            .AsBuilder(chatClient)
            .UseFunctionInvocation()
            .Build();

        var chatOptions = new ChatOptions { Tools = [AIFunctionFactory.Create(ConvertTemperature)] };

        IAsyncEnumerable<ChatResponseUpdate> response = chatClient.GetStreamingResponseAsync(
            "Convert 456 fahrenheit to celsius",
            chatOptions);
        
        await foreach (ChatResponseUpdate update in response)
        {
            Console.Write(update);
        }

        Console.WriteLine();
    }

    #region Function call with parameters helper

    /// <summary>
    /// Helper function for AI that calculates from Celsius to Fahrenheit or vice-versa.
    /// </summary>
    /// <returns>The final formatted conversion of values.</returns>
    [Description("Convert a temperature between Celsius and Fahrenheit")]
    private static string ConvertTemperature(
        [Description("The temperature value to convert")] double value,
        [Description("The unit to convert from: 'C' for Celsius, 'F' for Fahrenheit")] string fromUnit)
    {
        if (fromUnit.Equals("C", StringComparison.CurrentCulture))
        {
            double fahrenheit = (value * 9 / 5) + 32;
            return $"{value}°C = {fahrenheit:F1}°F";
        }

        double celsius = (value - 32) * 5 / 9;
        return $"{value}°F = {celsius:F1}°C";
    }

    #endregion

    public static async Task MultipleFunctions(IConfiguration configuration, string environment)
    {
        IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "granite4:350m");

        chatClient = ChatClientBuilderChatClientExtensions
            .AsBuilder(chatClient)
            .UseFunctionInvocation()
            .Build();

        var weatherTools = new WeatherTools(_httpClient, configuration, environment);
        var deviceSpecsTools = new DeviceSpecsTools(_httpClient, configuration, environment);
        var searchGamesTools = new SearchGamesTools(_httpClient, configuration, environment);

        var chatOptions = new ChatOptions 
        { 
            Tools = [
                AIFunctionFactory.Create(weatherTools.GetWeatherAsync),
                AIFunctionFactory.Create(deviceSpecsTools.GetDeviceSpecsAsync),
                AIFunctionFactory.Create(searchGamesTools.SearchGamesAsync)
            ]
        };

        var chatHistory = new List<ChatMessage>()
        {
            new ChatMessage(ChatRole.System, 
                "Using tools/function calling, you have access to weather, device specs, and game search. " +
                "Keep your responses concise (1-4 sentences only if it's enough).")
        };
        
        while (true)
        {
            Console.Write("Q: ");
            chatHistory.Add(new ChatMessage(ChatRole.User, Console.ReadLine()));

            var updates = new List<ChatResponseUpdate>();
            await foreach (ChatResponseUpdate update in chatClient.GetStreamingResponseAsync(chatHistory, chatOptions))
            {
                Console.Write(update.Text);
                updates.Add(update);
            }
            Console.WriteLine();
            
            chatHistory.AddMessages(updates);
        }
    }
    
    public static async Task MultiplePracticalFunctions() // Real-world example
    {
        IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "granite4:350m");

        chatClient = ChatClientBuilderChatClientExtensions
            .AsBuilder(chatClient)
            .UseFunctionInvocation()
            .Build();

        var chatOptions = new ChatOptions
        {
            Tools = [
                AIFunctionFactory.Create(GetCurrentTime),
                AIFunctionFactory.Create(CalculateTip),
                AIFunctionFactory.Create(ConvertCurrency)
            ]
        };

        Console.WriteLine(await chatClient.GetResponseAsync(
            "My dinner bill is $85. What's a good tip?", chatOptions));

        Console.WriteLine(await chatClient.GetResponseAsync(
            "How much is 500 euros in Japanese yen?", chatOptions));

        Console.WriteLine(await chatClient.GetResponseAsync(
            "What time is it Tokyo right now?", chatOptions));
    }

    #region Real-world example helpers

    [Description("Get the current date and time in a specific timezone")]
    private static string GetCurrentTime(string timezone = "UTC")
    {
        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
        DateTime time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

        return $"Current time is in {timezone}: {time:f}";
    }

    [Description("Calculate the tip amount for a bill")]
    private static string CalculateTip(double billAmount, double tipPercentage = 18)
    {
        double tip = billAmount * (tipPercentage / 100);
        double total = billAmount + tip;

        return $"Bill: {billAmount:F2}, Tip: ({tipPercentage}%): {tip:F2}, Total {total:F2}";
    }

    [Description("Convert between currencies using exchange rates")]
    private static string ConvertCurrency(double amount, string from, string to)
    {
        // Simplified - in reality, you'd call an exchange rate API
        var rates = new Dictionary<string, double>
        {
            ["USD"] = 1.0,
            ["EUR"] = 0.85,
            ["GBP"] = 0.73,
            ["JPY"] = 110.0
        };

        double usdAmount = amount / rates[from];
        double result = usdAmount * rates[to];

        return $"{amount} {from} = {result:F2} {to}";
    }

    #endregion
}
