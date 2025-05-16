# Model Context Protocol and Semantic Kernel


![I stand with Israel](./images/IStandWithIsrael.png)

## Hire Me

Please send [email](mailto:kingdavidconsulting@gmail.com) if you consider hiring me.

[![buymeacoffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/vyve0og)

## Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!

**Model Context Protocol (MCP)** is an emerging standard designed to bridge the gap between AI models and real-world data, tools, and services. Developed by Anthropic in late 2024, MCP aims to solve the problem of connecting large language models (LLMs) and AI agents to diverse and dynamic data sources securely and seamlessly.

**Key Points:**
1. **Purpose**: MCP is intended to provide a universal, open standard for connecting AI systems with data sources, replacing fragmented integrations with a single protocol.
2. **Architecture**: MCP follows a client-server architecture with components such as Host (AI-powered application), Client (manages connection to MCP server), Server (exposes capabilities over MCP protocol), and Base protocol (standardized messaging layer using JSON-RPC 2.0).
3. **Functionality**: MCP enables AI applications to invoke functions, fetch data, and utilize prompts from any compliant tool, database, or service through a single, secure interface.
4. **Adoption**: Major companies like OpenAI, Google, Microsoft, Replit, and Zapier have adopted MCP. A growing library of pre-built MCP connectors is emerging, including support from Docker.
5. **Use Cases**: MCP can be used for customer support chatbots, enterprise AI search, developer tools, and AI agents, among other applications.
6. **Future**: MCP is expected to become a foundational communications layer for AI systems, similar to what HTTP did for the web, enabling deeply integrated, context-aware, and action-capable AI systems.

MCP represents a significant step towards standardizing AI integration, making it easier for developers to connect AI models to various data sources and tools, ultimately unlocking the full potential of AI in various domains.

## Configuration

This application supports multiple LLM providers that can be configured via environment variables. To get started:

1. Copy the `.env.sample` file to `.env` in the src directory
2. Configure your preferred LLM provider

### Supported LLM Providers

#### OpenAI

To use OpenAI as your LLM provider:

```bash
# Set OpenAI as your provider
LLM__PROVIDER="OpenAI"

# Configure OpenAI credentials
OPENAI__APIKEY="your-openai-api-key"
OPENAI__CHATMODELID="gpt-4o"  # or any other model like gpt-4-turbo, gpt-3.5-turbo
```

#### Azure OpenAI

To use Azure OpenAI as your LLM provider:

```bash
# Set Azure OpenAI as your provider
LLM__PROVIDER="AzureOpenAI"

# Configure Azure OpenAI credentials
AZUREOPENAI__DEPLOYMENTNAME="gpt-4o"  # Your deployment name in Azure
AZUREOPENAI__ENDPOINT="https://your-resource-name.openai.azure.com"
AZUREOPENAI__APIKEY="your-azure-openai-api-key"
```

#### Ollama (Local Models)

To use Ollama for running local models:

```bash
# Set Ollama as your provider
LLM__PROVIDER="Ollama"

# Configure Ollama
OLLAMA__MODELNAME="mistral"  # or llama3, phi3, etc.
OLLAMA__ENDPOINT="http://localhost:11434"  # Default Ollama endpoint
```

Ensure Ollama is installed and running on your machine before using this option. You can download Ollama from [https://ollama.com/](https://ollama.com/).

## Running the Application

To run the application:

```bash
cd src
dotnet run
```

The application will use the LLM provider specified in your environment configuration.

## Inspired by
- [semantic-kernel-playwright-mcp](https://github.com/akshaykokane/semantic-kernel-playwright-mcp)
- [Model Context Protocol Sample](https://github.com/microsoft/semantic-kernel/blob/7a19ae350eaff5746ace52d2c894a9975d82ba59/dotnet/samples/Demos/ModelContextProtocol/README.md)