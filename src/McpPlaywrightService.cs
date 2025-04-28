// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol;

public class McpPlaywrightService : BackgroundService
{
    private readonly Kernel _kernel;
    private readonly ILogger<McpPlaywrightService> _logger;

    public McpPlaywrightService(Kernel kernel, ILogger<McpPlaywrightService> logger)
    {
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("McpPlaywrightService starting...");
        
        try
        {
            await ExecuteMCPlaywrightClient(_kernel, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while executing MCP Playwright client");
        }
    }

    private async Task ExecuteMCPlaywrightClient(Kernel kernel, CancellationToken cancellationToken)
    {
        // Create an MCPClient for the Playwright server
        _logger.LogInformation("Initializing MCP Playwright client...");
        var mcpPlaywrightClient = await McpDotNetExtensions.GetMCPClientForPlaywright().ConfigureAwait(false);

        // Retrieve the list of tools available on the Playwright server
        var tools = await mcpPlaywrightClient.ListToolsAsync().ConfigureAwait(false);
        foreach (var tool in tools.Tools)
        {
            _logger.LogInformation("{ToolName}: {ToolDescription}", tool.Name, tool.Description);
        }

        // Add the MCP tools as Kernel functions
        var functions = await mcpPlaywrightClient.MapToFunctionsAsync().ConfigureAwait(false);
        kernel.Plugins.AddFromFunctions("Browser", functions);

        // Enable automatic function calling
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        // Test using Playwright tools
        var prompt = "Summarize AI news for me related to MCP on bing news. Open first link and summarize content";
        _logger.LogInformation("Executing prompt: {Prompt}", prompt);
        var result = await kernel.InvokePromptAsync(prompt, new(executionSettings)).ConfigureAwait(false);
        _logger.LogInformation("\n\n{Prompt}\n{Result}", prompt, result);
    }
}
