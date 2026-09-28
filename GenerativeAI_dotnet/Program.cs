using Microsoft.Extensions.AI;
using OllamaSharp;

namespace GenerativeAI_dotnet;

public class Program
{
    public static async Task Main(string[] args)
    {
        IChatClient client = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2:1b");

        ChatResponse response = await client.GetResponseAsync("What is Hello world? (answer in one sentence)");
        Console.WriteLine(response);
    }
}
