# GenerativeAI_dotnet

By Mark Jason T. Galang ([@markjasongalang](https://github.com/markjasongalang))

## Overview

Before, I wasn't really interested in making AI-powered tools, but I came across a hackathon post on social media and the aim is to build an AI-powered tool (the winner will receive a cash prize). This led me into researching this topic and here we are.

Since this is my first time integrating AI into an app, I knew I needed to have solid fundamentals, and luckily, I found this repo: [Generative AI for Beginners .NET](https://github.com/microsoft/Generative-AI-for-beginners-dotnet).

Currently, I'm following along with this guide and I hope to become better at AI-integration while still making efficient and performant apps/systems.

## Ollama (local model)

I tried the [llama2](https://ollama.com/library/llama2) model first and prompted "explain docker in one sentence", but it was too slow (took a few minutes) to generate the response, so I switched to [llama3.2](https://ollama.com/library/llama3.2) with **1B** parameter size then it generated the answer within a few seconds.

Refer to <https://computingforgeeks.com/ollama-models-cheat-sheet/>

My PC specs:

| Item                            | Value                                                                                |
| :------------------------------ | :----------------------------------------------------------------------------------- |
| OS Name                         | Microsoft Windows 11 Pro                                                             |
| System Type                     | x64-based PC                                                                         |
| Processor                       | AMD Ryzen 5 5600G with Radeon Graphics, 3901 Mhz, 6 Core(s), 12 Logical Processor(s) |
| Installed Physical Memory (RAM) | 16.0 GB                                                                              |

Sample response:
```
Docker is a containerization platform that allows developers to package, ship, and run applications in containers,
enabling efficient and portable deployment of software applications.
```

You can check out the Ollama models here: <https://ollama.com/search>

**EDIT:** When I arrived at [Streaming and Structured Output](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/02-GenerativeAITechniques/02-streaming-structured-output.md) part,
specifically, the sentiment example, I wasn't satisfied with the response of [llama3.2:1b](https://ollama.com/library/llama3.2), so after searching, I replaced it with [gemma3:1b](https://ollama.com/library/gemma3)
which is faster, lighter (only 815 MB), and a bit more accurate since it matched some of the neutral responses in the example.

## Get Started

### Download image from Docker Hub registry

```docker
docker pull ollama/ollama
```

Link: <https://hub.docker.com/r/ollama/ollama>

### Run container

```docker
docker run --rm -d -v ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama
```

### See if Ollama is running

```powershell
curl http://localhost:11434
```

Response:
```
StatusCode        : 200
StatusDescription : OK
Content           : Ollama is running
RawContent        : HTTP/1.1 200 OK
                    Content-Length: 17
                    Content-Type: text/plain; charset=utf-8
                    Date: Mon, 28 Sep 2026 08:51:16 GMT

                    Ollama is running
Forms             : {}
Headers           : {[Content-Length, 17], [Content-Type, text/plain; charset=utf-8], [Date, Mon, 28 Sep 2026 08:51:16
                    GMT]}
Images            : {}
InputFields       : {}
Links             : {}
ParsedHtml        : mshtml.HTMLDocumentClass
RawContentLength  : 17
```

## Pull a Model from the Ollama model registry

```docker
ollama pull gemma3:1b
```

### Run a model locally

```docker
docker exec -it ollama ollama run gemma3:1b
```

### Implementation

I created a Console app project in Visual Studio for this tutorial.

We need to install these NuGet packages first:

- [Microsoft.Extensions.AI (MEAI)](https://www.nuget.org/packages/Microsoft.Extensions.AI/) 
- [OllamaSharp](https://www.nuget.org/packages/OllamaSharp)

Refer to <https://learn.microsoft.com/en-us/dotnet/ai/ichatclient>

#### System Message

Apparently, this is the most powerful tool for controlling AI behavior. It's the first message in our conversation with the AI model and in here, we define the expertise, scope, and boundaries.

Refer to <https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/02-GenerativeAITechniques/01-text-completions-chat.md>

On the guide, the first example of this concept was to set the expertise of the AI model as a *Senior .NET Developer*. Honestly, I did my best this whole afternoon, but I couldn't fully restrict the AI model to only respond strictly to .NET topics and don't entertain non-.NET topics.

This is the system message that I was tinkering the whole time:
```csharp
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
```

> Note: The model "remembers" because we're tracking the whole conversation by maintaining an entire history (list) so we could send this with each request to the model.

Below are my actual conversations with the AI model:

**Scenario 1:** Its first answer blocks the non-.NET question, however, proceeds to answer it on the next prompt.

```
Q: what is cooking
I only cater to .NET topics.

Q: what is cooking
Cooking is the process of preparing food, typically by applying heat, moisture, or other methods to transform ingredients into a palatable dish.
```

**Scenario 2:** AI trying to answer the unrelated topic with code.

```
Q: what is cooking
I only cater to .NET topics.

Q: what is cooking
Cooking is the process of preparing food in a way that makes it safe to eat and enjoyable to consume. It involves a combination of culinary arts, chemistry, and nutrition, and can be done through various methods such as roasting, boiling, frying, and more. The goal of cooking is to create a dish that is both nourishing and palatable.

Q: what is cooking
using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Cooking is the process of preparing food in a way that makes it safe to eat and enjoyable to consume.");
    }
}

Q: what is cooking
I'm only catering to .NET topics. There is no direct equivalent to "cooking" in programming, but if you're looking for information on a specific cooking-related concept, I can try to provide a generic answer.

Q: what is cooking
using System;
using System.Collections.Generic;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Cooking is the process of preparing food to be consumed by humans.");
    }
}
```

**Scenario 3:** AI sneakily answering the unrelated topic/question

```
Q: what is cooking
I'm only catering to .NET topics. Cooking is a domestic science that involves the preparation and presentation of food, and it's not a programming concept.

Q: what is cooking
I cannot provide information on cooking.

Q: what is cooking
The term "cooking" can refer to various activities related to preparing and presenting food, such as baking, broiling, grilling, roasting, sautéing, and more. However, in a broader sense, cooking can also refer to the process of preserving or preparing food through various methods, including canning, freezing, and dehydrating.

In .NET, we have a similar concept called "cooking" where we can apply algorithms and data structures to prepare and transform data into a usable format. For example, we can use LINQ to query and manipulate data, or use data structures like lists and dictionaries to store and process data.

Q: what is cooking
I can't provide information on cooking.

Q: what is cooking
I can't answer that.

Q: what is cooking
The programming language C# is a member of the .NET family of products, which are developed by Microsoft.

Q: what is cooking
I'm only catering to .NET topics. Cooking is not a programming concept.

Q: what is jogging
I'm only catering to .NET topics. Jogging is a form of aerobic exercise, but it's not related to programming or .NET.

Q: what is an airplane
I'm only catering to .NET topics. An airplane is a type of aircraft, and while it's a critical mode of transportation, it's not related to programming or .NET.
```

As of now (Sept. 29, 2026), I haven't formulated the correct system message to restrict the AI model to only answer .NET topics.
