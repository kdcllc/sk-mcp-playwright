﻿// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) => 
            {
                var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                      .AddUserSecrets<Program>()
                      .AddEnvironmentVariables();
            })
            .ConfigureLogging((hostContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
            })
            .ConfigureServices((hostContext, services) =>
            {
                // Configure Kernel
                var kernelBuilder = Kernel.CreateBuilder();
                
                // Configure OpenAI or Azure OpenAI
                var config = hostContext.Configuration;
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
                
                // Add Kernel to DI container
                services.AddSingleton(kernelBuilder.Build());
                
                // Add hosted service
                services.AddHostedService<McpPlaywrightService>();
            })
            .RunConsoleAsync();
    }
}