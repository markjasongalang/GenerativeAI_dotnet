using System.Text;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace GenerativeAI_dotnet;

public class Program
{
    public static async Task Main(string[] args)
    {
        IChatClient client = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2:1b");

        await SentimentAnalysis(client);
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
}
