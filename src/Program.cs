
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

    var openAiApiKey = config["OpenAI:ApiKey"];
    if (!string.IsNullOrEmpty(openAiApiKey))
    {
        kernelBuilder.Services.AddOpenAIChatCompletion(
            serviceId: "openai",
            modelId: config["OpenAI:ChatModelId"] ?? "gpt-4o",
            apiKey: openAiApiKey);
    }
    else
    {
        var azureOpenAIEndpoint = config["AzureOpenAI:Endpoint"];
        var azureOpenAIApiKey = config["AzureOpenAI:ApiKey"];

        if (string.IsNullOrEmpty(azureOpenAIEndpoint) || string.IsNullOrEmpty(azureOpenAIApiKey))
        {
            throw new InvalidOperationException("AzureOpenAI:Endpoint and AzureOpenAI:ApiKey must be set in configuration");
        }

        kernelBuilder.AddAzureOpenAIChatCompletion(
            deploymentName: config["AzureOpenAI:DeploymentName"] ?? "gpt-4o",
            endpoint: azureOpenAIEndpoint,
            apiKey: azureOpenAIApiKey);
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
