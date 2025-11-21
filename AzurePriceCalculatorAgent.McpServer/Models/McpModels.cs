namespace AzurePriceCalculatorAgent.McpServer.Models;

/// <summary>
/// Tool definition for MCP
/// </summary>
public class McpTool
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public object InputSchema { get; set; } = new();
}

/// <summary>
/// Tools list response
/// </summary>
public class ToolsListResponse
{
    public List<McpTool> Tools { get; set; } = new();
}
