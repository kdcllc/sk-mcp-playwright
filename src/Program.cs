using DotNetEnv.Configuration;

using Microsoft.SemanticKernel;

var hostBuilder = Host.CreateApplicationBuilder(args);
hostBuilder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
hostBuilder.Configuration.AddJsonFile($"appsettings.{hostBuilder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
hostBuilder.Configuration.AddUserSecrets<Program>();
hostBuilder.Configuration.AddDotNetEnv();
hostBuilder.Configuration.AddEnvironmentVariables();

hostBuilder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddConfiguration(hostBuilder.Configuration.GetSection("Logging"));
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

hostBuilder.Services.AddSingleton(sp =>
{
    var kernelBuilder = Kernel.CreateBuilder();
    var config = sp.GetRequiredService<IConfiguration>();

    var llmProvider = config["LLM:Provider"] ?? "AzureOpenAI";

    switch (llmProvider.ToUpperInvariant())
    {
        case "OPENAI":
            var openAiApiKey = config["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(openAiApiKey))
            {
                throw new InvalidOperationException("OpenAI:ApiKey must be set in configuration when LLM:Provider is OpenAI");
            }

            kernelBuilder.Services.AddOpenAIChatCompletion(
                serviceId: "openai",
                modelId: config["OpenAI:ChatModelId"] ?? "gpt-4o",
                apiKey: openAiApiKey);
            break;

        case "AZUREOPENAI":
            var azureOpenAIEndpoint = config["AzureOpenAI:Endpoint"];
            var azureOpenAIApiKey = config["AzureOpenAI:ApiKey"];

            if (string.IsNullOrEmpty(azureOpenAIEndpoint) || string.IsNullOrEmpty(azureOpenAIApiKey))
            {
                throw new InvalidOperationException("AzureOpenAI:Endpoint and AzureOpenAI:ApiKey must be set in configuration when LLM:Provider is AzureOpenAI");
            }

            kernelBuilder.AddAzureOpenAIChatCompletion(
                deploymentName: config["AzureOpenAI:DeploymentName"] ?? "gpt-4o",
                endpoint: azureOpenAIEndpoint,
                apiKey: azureOpenAIApiKey);
            break;

        case "OLLAMA":
            var ollamaEndpoint = config["Ollama:Endpoint"] ?? "http://localhost:11434";
            var ollamaModelName = config["Ollama:ModelName"] ?? "mistral";

            kernelBuilder.AddOpenAIChatCompletion(
                        modelId: ollamaModelName,
                        endpoint: new Uri(ollamaEndpoint),
                        apiKey: "apikey");

            // can be used with this package <PackageReference Include="Microsoft.SemanticKernel.Connectors.Ollama" Version="1.47.0-alpha" />
            //#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            //            kernelBuilder.AddOllamaChatCompletion(
            //                modelId: ollamaModelName,
            //                endpoint: new Uri(ollamaEndpoint));
            //#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            break;

        default:
            throw new InvalidOperationException($"Unsupported LLM provider: {llmProvider}. Supported values are: OpenAI, AzureOpenAI, Ollama");
    }

    return kernelBuilder.Build();
});

hostBuilder.Services.AddSingleton<McpPlaywrightService>();

var app = hostBuilder.Build();

var hostLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

var mcpclient = app.Services.GetRequiredService<McpPlaywrightService>();

// create comnbine cancellation token for graceful shutdown
var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(hostLifetime.ApplicationStopping);
var cancellationToken = cancellationTokenSource.Token;

await mcpclient.ExecuteAsync(cancellationToken);
