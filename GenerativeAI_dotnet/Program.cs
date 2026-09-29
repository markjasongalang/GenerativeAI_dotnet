using System.Text;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace GenerativeAI_dotnet;

public class Program
{
    public static async Task Main(string[] args)
    {
        IChatClient client = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2:1b");


    }

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
}
