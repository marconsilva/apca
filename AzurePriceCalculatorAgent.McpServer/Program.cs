using AzurePriceCalculatorAgent.McpServer.Models;
using AzurePriceCalculatorAgent.McpServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddOpenApi();

// Register services
builder.Services.AddScoped<DiagramAnalysisService>();
builder.Services.AddScoped<AzurePricingService>();
builder.Services.AddHttpClient<AzurePricingService>();

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.MapGet("/", () => Results.Ok(new
{
    service = "Azure Price Calculator Agent - MCP Server",
    version = "1.0.0",
    endpoints = new[]
    {
        "/mcp/tools",
        "/mcp/extract-resources",
        "/mcp/get-pricing",
        "/mcp/calculate-cost"
    }
}))
.WithName("Root")
.WithDescription("MCP Server root endpoint");

// MCP Tool: List available tools
app.MapGet("/mcp/tools", () =>
{
    var tools = new List<McpTool>
    {
        new McpTool
        {
            Name = "extract_azure_resources_from_diagram",
            Description = "Analyzes an architecture diagram image and extracts a list of Azure resources present in the architecture. Returns a structured list of Azure services with their types and suggested quantities/configurations.",
            InputSchema = new Dictionary<string, object>
            {
                ["type"] = "object",
                ["properties"] = new Dictionary<string, object>
                {
                    ["image_data"] = new Dictionary<string, object>
                    {
                        ["type"] = "string",
                        ["description"] = "Base64-encoded image data or URL of the architecture diagram"
                    },
                    ["image_type"] = new Dictionary<string, object>
                    {
                        ["type"] = "string",
                        ["enum"] = new[] { "base64", "url" },
                        ["description"] = "Type of image input (base64 or url)",
                        ["default"] = "base64"
                    }
                },
                ["required"] = new[] { "image_data" }
            }
        },
        new McpTool
        {
            Name = "get_azure_resource_pricing",
            Description = "Gets pricing information for specified Azure resources using the Azure Retail Prices API. Returns detailed pricing for each resource including per-hour, per-month rates, and currency information.",
            InputSchema = new Dictionary<string, object>
            {
                ["type"] = "object",
                ["properties"] = new Dictionary<string, object>
                {
                    ["resources"] = new Dictionary<string, object>
                    {
                        ["type"] = "array",
                        ["description"] = "List of Azure resources to get pricing for"
                    }
                },
                ["required"] = new[] { "resources" }
            }
        },
        new McpTool
        {
            Name = "calculate_total_cost",
            Description = "Calculates total monthly and yearly costs for a list of Azure resources with pricing. Takes pricing data from get_azure_resource_pricing and computes totals.",
            InputSchema = new Dictionary<string, object>
            {
                ["type"] = "object",
                ["properties"] = new Dictionary<string, object>
                {
                    ["pricing_data"] = new Dictionary<string, object>
                    {
                        ["type"] = "array",
                        ["description"] = "Array of resources with pricing information"
                    }
                },
                ["required"] = new[] { "pricing_data" }
            }
        }
    };

    return Results.Ok(new ToolsListResponse { Tools = tools });
})
.WithName("ListTools")
.WithDescription("List all available MCP tools");

// MCP Tool: Extract Azure resources from diagram
app.MapPost("/mcp/extract-resources", async (
    DiagramAnalysisRequest request,
    DiagramAnalysisService analysisService) =>
{
    var result = await analysisService.AnalyzeArchitectureDiagramAsync(
        request.ImageData,
        request.ImageType ?? "base64");
    
    return Results.Ok(result);
})
.WithName("ExtractResources")
.WithDescription("Extract Azure resources from architecture diagram");

// MCP Tool: Get Azure resource pricing
app.MapPost("/mcp/get-pricing", async (
    List<AzureResource> resources,
    AzurePricingService pricingService) =>
{
    var result = await pricingService.GetResourcePricingAsync(resources);
    return Results.Ok(result);
})
.WithName("GetPricing")
.WithDescription("Get pricing information for Azure resources");

// MCP Tool: Calculate total cost
app.MapPost("/mcp/calculate-cost", async (
    List<ResourcePricing> pricingData,
    AzurePricingService pricingService) =>
{
    var result = await pricingService.CalculateTotalCostAsync(pricingData);
    return Results.Ok(result);
})
.WithName("CalculateCost")
.WithDescription("Calculate total cost from pricing data");

app.MapDefaultEndpoints();

app.Run();

// Request models
record DiagramAnalysisRequest(string ImageData, string? ImageType);
