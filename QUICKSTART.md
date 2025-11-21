# Azure Price Calculator Agent - Quick Start Guide

This guide will help you get the Azure Price Calculator Agent up and running quickly.

## Option 1: Run Locally with .NET Aspire (Recommended for Development)

### Prerequisites
- .NET 10 SDK
- Docker Desktop

### Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/marconsilva/apca.git
   cd apca
   ```

2. **Run the application**
   ```bash
   cd AzurePriceCalculatorAgent.AppHost
   dotnet run
   ```

3. **Access the applications**
   - Open the Aspire Dashboard URL shown in the console (typically http://localhost:15888)
   - Click on the "webfrontend" endpoint to access the web interface
   - Navigate to "Price Calculator" in the menu

## Option 2: Deploy to Azure with Azure Developer CLI

### Prerequisites
- Azure subscription
- Azure Developer CLI (azd)
- Azure CLI

### Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/marconsilva/apca.git
   cd apca
   ```

2. **Login to Azure**
   ```bash
   azd auth login
   ```

3. **Deploy to Azure**
   ```bash
   azd up
   ```

4. **Access your application**
   - The deployment will output the URL for your web application
   - Open the URL in your browser

## Using the Price Calculator

### Manual Entry

1. Navigate to the "Price Calculator" page
2. Select a service (e.g., Virtual Machines)
3. Enter the SKU name (e.g., Standard_D2s_v3)
4. Select a region
5. Enter the quantity
6. Click "Add Resource"
7. Repeat for all resources
8. Click "Calculate Pricing"

### Example Resources

Click "Load Example Resources" to quickly test with:
- 2x Virtual Machines (Standard_D2s_v3) in East US
- 1x Azure SQL Database (S3) in East US
- 1x Storage Account (Standard_LRS) in East US

## Using the MCP Server API

The MCP Server exposes REST APIs that can be called directly:

### Get Available Tools
```bash
curl https://your-mcp-server-url/mcp/tools
```

### Get Pricing for Resources
```bash
curl -X POST https://your-mcp-server-url/mcp/get-pricing \
  -H "Content-Type: application/json" \
  -d '[
    {
      "serviceName": "Virtual Machines",
      "skuName": "Standard_D2s_v3",
      "region": "eastus",
      "quantity": 2
    }
  ]'
```

### Calculate Total Cost
```bash
curl -X POST https://your-mcp-server-url/mcp/calculate-cost \
  -H "Content-Type: application/json" \
  -d '[
    {
      "resourceName": "Virtual Machines - Standard_D2s_v3",
      "hourlyCost": 0.096,
      "quantity": 2,
      "currency": "USD"
    }
  ]'
```

## Using with GitHub Copilot

If you're using GitHub Copilot, you can ask questions like:

- "How much would 3 Standard_D2s_v3 VMs cost in East US?"
- "Calculate the monthly cost for a Standard SQL Database in West Europe"
- "What's the yearly cost for 5 TB of Standard LRS storage?"

The agent will use the MCP Server tools to provide accurate, real-time pricing information.

## Troubleshooting

### Issue: Can't connect to services
- Ensure Docker Desktop is running
- Check that all services are healthy in the Aspire Dashboard

### Issue: Pricing data not loading
- Verify internet connectivity (MCP Server needs to reach Azure Retail Prices API)
- Check service logs in Aspire Dashboard

### Issue: Deployment fails
- Ensure you're logged into Azure (`azd auth login`)
- Verify you have sufficient permissions in your Azure subscription
- Check that the resource names are available in your target region

## Next Steps

- Explore the [full README](README.md) for detailed documentation
- Check out the [architecture diagram](docs/architecture.md) (if available)
- Review the [API documentation](docs/api.md) (if available)
- Contribute to the project (see [CONTRIBUTING.md](CONTRIBUTING.md))

## Support

For issues and questions:
- Open an issue on GitHub
- Check existing issues for solutions
- Review the FAQ section in the README
