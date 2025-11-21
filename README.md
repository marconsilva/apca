# Azure Price Calculator Agent

An intelligent Azure pricing calculator agent built with .NET Aspire, MCP (Model Context Protocol) Server, and Azure Container Apps. This solution helps you analyze Azure architecture diagrams, extract resources, and calculate accurate cost estimates using real-time Azure pricing data.

## Features

🎯 **Architecture Diagram Analysis** - Upload architecture diagrams and automatically detect Azure resources

💰 **Real-time Pricing** - Get current pricing from Azure Retail Prices API

📊 **Cost Breakdown** - Detailed monthly and yearly cost estimates per resource

🤖 **GitHub Copilot Integration** - Use as an AI agent with GitHub Copilot

☁️ **Azure Container Apps** - Production-ready deployment on Azure

🚀 **.NET Aspire** - Modern cloud-native orchestration

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Azure Price Calculator Agent              │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Web App    │  │  MCP Server  │  │ API Service  │      │
│  │   (Blazor)   │  │   (Tools)    │  │  (Backend)   │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│         │                  │                  │              │
│         └──────────────────┴──────────────────┘              │
│                            │                                 │
│                   ┌────────▼────────┐                       │
│                   │  .NET Aspire    │                       │
│                   │   AppHost       │                       │
│                   └─────────────────┘                       │
└─────────────────────────────────────────────────────────────┘
                            │
                   ┌────────▼────────┐
                   │ Azure Container │
                   │      Apps       │
                   └─────────────────┘
```

## Projects

### 1. **AzurePriceCalculatorAgent.Web**
Blazor Server web application providing the user interface for:
- Uploading architecture diagrams
- Viewing detected Azure resources
- Displaying cost estimates and breakdowns

### 2. **AzurePriceCalculatorAgent.McpServer**
MCP (Model Context Protocol) Server with three main tools:

- **extract_azure_resources_from_diagram**: Analyzes diagrams to detect Azure resources
- **get_azure_resource_pricing**: Fetches pricing from Azure Retail Prices API
- **calculate_total_cost**: Computes total monthly and yearly costs

### 3. **AzurePriceCalculatorAgent.ApiService**
Backend API service for additional functionality

### 4. **AzurePriceCalculatorAgent.AppHost**
.NET Aspire orchestration host that manages all services

### 5. **AzurePriceCalculatorAgent.ServiceDefaults**
Shared service defaults for health checks, telemetry, and service discovery

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for local development)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli) (for deployment)
- [Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd) (for deployment)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# extension

## Getting Started

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/marconsilva/apca.git
   cd apca
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run with .NET Aspire**
   ```bash
   cd AzurePriceCalculatorAgent.AppHost
   dotnet run
   ```

4. **Access the applications**
   - Aspire Dashboard: `http://localhost:15888` (or check console output)
   - Web App: Check Aspire Dashboard for the web frontend URL
   - MCP Server: Check Aspire Dashboard for the MCP Server URL

### Run Individual Projects

**MCP Server:**
```bash
cd AzurePriceCalculatorAgent.McpServer
dotnet run
```

**Web Application:**
```bash
cd AzurePriceCalculatorAgent.Web
dotnet run
```

## Deployment to Azure

This project is **Azure Developer CLI (azd) ready** and can be deployed to Azure Container Apps with a single command.

### Initial Setup

1. **Login to Azure**
   ```bash
   azd auth login
   ```

2. **Initialize the environment**
   ```bash
   azd init
   ```
   - When prompted, use the environment name of your choice (e.g., `apca-prod`)
   - Select your Azure subscription
   - Select your preferred Azure region (e.g., `eastus`)

### Deploy to Azure

```bash
azd up
```

This command will:
- Create a new resource group
- Provision Azure Container Apps Environment
- Create Azure Container Registry
- Set up Application Insights for monitoring
- Build and push container images
- Deploy all services to Azure Container Apps

### Infrastructure Details

The deployment creates:
- **Resource Group**: `rg-{environment-name}`
- **Container Apps Environment**: For hosting all services
- **Container Registry**: For storing container images
- **Log Analytics Workspace**: For logging and monitoring
- **Application Insights**: For telemetry and diagnostics
- **3 Container Apps**: Web, MCP Server, and API Service

### Update Deployment

After making changes to your code:

```bash
azd deploy
```

### View Deployed Resources

```bash
azd show
```

### Access Deployed Application

After deployment completes, azd will output the URLs for your services:
- **Web App**: `https://ca-web-{resourceToken}.{region}.azurecontainerapps.io`
- **MCP Server**: `https://ca-mcpserver-{resourceToken}.{region}.azurecontainerapps.io`

### Clean Up Resources

To delete all Azure resources:

```bash
azd down
```

## MCP Server API

The MCP Server exposes the following endpoints:

### GET /mcp/tools
Lists all available tools with their schemas.

### POST /mcp/extract-resources
Extract Azure resources from an architecture diagram.

**Request Body:**
```json
{
  "imageData": "base64-encoded-image-or-url",
  "imageType": "base64"
}
```

**Response:**
```json
{
  "status": "success",
  "message": "Architecture diagram analyzed successfully",
  "detectedResources": [
    {
      "serviceName": "Virtual Machines",
      "skuName": "Standard_D2s_v3",
      "quantity": 2,
      "region": "eastus",
      "notes": "Web servers"
    }
  ]
}
```

### POST /mcp/get-pricing
Get pricing information for Azure resources.

**Request Body:**
```json
[
  {
    "serviceName": "Virtual Machines",
    "skuName": "Standard_D2s_v3",
    "region": "eastus",
    "quantity": 2
  }
]
```

**Response:**
```json
{
  "status": "success",
  "resourceCount": 1,
  "pricingData": [
    {
      "resourceName": "Virtual Machines - Standard_D2s_v3",
      "hourlyCost": 0.096,
      "monthlyCost": 140.16,
      "yearlyCost": 1681.92,
      "currencyCode": "USD"
    }
  ]
}
```

### POST /mcp/calculate-cost
Calculate total costs from pricing data.

**Request Body:**
```json
[
  {
    "resourceName": "Virtual Machines - Standard_D2s_v3",
    "hourlyCost": 0.096,
    "quantity": 2,
    "currency": "USD"
  }
]
```

**Response:**
```json
{
  "status": "success",
  "currency": "USD",
  "summary": {
    "totalHourlyCost": 0.192,
    "totalMonthlyCost": 280.32,
    "totalYearlyCost": 3363.84
  },
  "breakdown": [...]
}
```

## GitHub Copilot Integration

This repository includes GitHub Copilot Agent configuration. The agent can:

1. Analyze architecture diagrams you share
2. Identify Azure resources from descriptions
3. Provide cost estimates
4. Suggest cost optimization strategies

### Using with GitHub Copilot

1. Open the repository in GitHub Copilot Chat
2. Ask questions like:
   - "Analyze this architecture diagram and estimate costs"
   - "How much would 3 VMs cost in East US?"
   - "What's the monthly cost for a Standard SQL Database in West Europe?"

The agent has access to the MCP Server tools and can provide real-time pricing information.

## Configuration

### Environment Variables

**MCP Server:**
- `APPLICATIONINSIGHTS_CONNECTION_STRING`: Application Insights connection string
- `ASPNETCORE_ENVIRONMENT`: Environment name (Development, Production)

**Web App:**
- `services__mcpserver__https__0`: MCP Server URL
- `services__apiservice__https__0`: API Service URL

## Development

### Project Structure

```
apca/
├── .github/
│   └── copilot-instructions.md          # GitHub Copilot agent configuration
├── infra/                                # Infrastructure as Code (Bicep)
│   ├── main.bicep                        # Main infrastructure template
│   ├── main.parameters.json              # Parameters file
│   ├── abbreviations.json                # Azure resource abbreviations
│   ├── core/                             # Core infrastructure modules
│   │   ├── host/
│   │   │   ├── container-apps-environment.bicep
│   │   │   └── container-registry.bicep
│   │   └── monitor/
│   │       └── monitoring.bicep
│   └── app/                              # Application-specific modules
│       ├── mcpserver.bicep
│       ├── apiservice.bicep
│       └── web.bicep
├── AzurePriceCalculatorAgent.AppHost/    # Aspire orchestration
├── AzurePriceCalculatorAgent.Web/        # Blazor web app
├── AzurePriceCalculatorAgent.McpServer/  # MCP Server with pricing tools
├── AzurePriceCalculatorAgent.ApiService/ # Backend API
├── AzurePriceCalculatorAgent.ServiceDefaults/ # Shared defaults
├── azure.yaml                            # Azure Developer CLI configuration
└── README.md
```

### Building

```bash
dotnet build
```

### Testing

```bash
dotnet test
```

### Code Formatting

The project uses standard .NET formatting. To format code:

```bash
dotnet format
```

## Technologies Used

- **.NET 10**: Latest .NET framework
- **.NET Aspire**: Cloud-native orchestration
- **Blazor**: Interactive web UI
- **Azure Container Apps**: Serverless container hosting
- **Azure Retail Prices API**: Real-time Azure pricing data
- **MCP (Model Context Protocol)**: Tool-based agent framework
- **Application Insights**: Monitoring and diagnostics
- **Bicep**: Infrastructure as Code
- **Azure Developer CLI**: Deployment automation

## Pricing Data Source

This application uses the official [Azure Retail Prices API](https://learn.microsoft.com/rest/api/cost-management/retail-prices/azure-retail-prices) to fetch current pricing information. The API provides:

- Up-to-date pricing for all Azure services
- Regional pricing variations
- SKU-specific details
- Multiple currencies support

## Limitations

- Architecture diagram analysis currently returns example data (requires Azure Computer Vision or GPT-4 Vision integration for full functionality)
- Pricing is based on standard pay-as-you-go rates
- Does not include discounts, reserved instances, or Azure Hybrid Benefit
- Estimates assume 24/7 usage

## Future Enhancements

- [ ] Integrate Azure Computer Vision for actual diagram analysis
- [ ] Support for reserved instance pricing
- [ ] Cost optimization recommendations
- [ ] Multi-cloud comparison (AWS, GCP)
- [ ] Export cost reports to Excel/PDF
- [ ] Historical pricing trends
- [ ] Budget alerts and notifications

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For issues and questions:
- Create an issue in the GitHub repository
- Contact: [Your contact information]

## Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Azure Container Apps Documentation](https://learn.microsoft.com/azure/container-apps/)
- [Azure Retail Prices API](https://learn.microsoft.com/rest/api/cost-management/retail-prices/azure-retail-prices)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/)
- [MCP Protocol](https://modelcontextprotocol.io/)

