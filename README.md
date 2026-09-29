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

### Run a model locally

```docker
docker exec -it ollama ollama run llama3.2
```

### Remove a model

```docker
docker exec -it ollama ollama rm llama2:latest
```

### Implementation

I created a Console app project in Visual Studio for this tutorial.

Apparently, we'll need to install these NuGet packages:

- [Microsoft.Extensions.AI (MEAI)](https://www.nuget.org/packages/Microsoft.Extensions.AI/) 
- [OllamaSharp](https://www.nuget.org/packages/OllamaSharp)

Refer to <https://learn.microsoft.com/en-us/dotnet/ai/ichatclient>
