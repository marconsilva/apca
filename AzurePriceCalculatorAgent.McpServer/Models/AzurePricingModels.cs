namespace AzurePriceCalculatorAgent.McpServer.Models;

/// <summary>
/// Represents an Azure resource with its configuration
/// </summary>
public class AzureResource
{
    public string ServiceName { get; set; } = string.Empty;
    public string SkuName { get; set; } = string.Empty;
    public string Region { get; set; } = "eastus";
    public int Quantity { get; set; } = 1;
    public string? Notes { get; set; }
}

/// <summary>
/// Represents pricing information for an Azure resource
/// </summary>
public class ResourcePricing
{
    public string ResourceName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string SkuName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal RetailPrice { get; set; }
    public decimal UnitPrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public decimal HourlyCost { get; set; }
    public decimal MonthlyCost { get; set; }
    public decimal YearlyCost { get; set; }
    public string? ProductName { get; set; }
    public string? MeterName { get; set; }
    public string? Error { get; set; }
}

/// <summary>
/// Result from extracting resources from diagram
/// </summary>
public class DiagramAnalysisResult
{
    public string Status { get; set; } = "success";
    public string Message { get; set; } = string.Empty;
    public List<AzureResource> DetectedResources { get; set; } = new();
    public string? Note { get; set; }
}

/// <summary>
/// Result from pricing analysis
/// </summary>
public class PricingAnalysisResult
{
    public string Status { get; set; } = "success";
    public int ResourceCount { get; set; }
    public List<ResourcePricing> PricingData { get; set; } = new();
    public string? Instructions { get; set; }
}

/// <summary>
/// Cost calculation result
/// </summary>
public class CostCalculationResult
{
    public string Status { get; set; } = "success";
    public string Currency { get; set; } = "USD";
    public CostSummary Summary { get; set; } = new();
    public List<ResourceCostBreakdown> Breakdown { get; set; } = new();
    public List<string> Notes { get; set; } = new();
}

/// <summary>
/// Summary of total costs
/// </summary>
public class CostSummary
{
    public decimal TotalHourlyCost { get; set; }
    public decimal TotalMonthlyCost { get; set; }
    public decimal TotalYearlyCost { get; set; }
}

/// <summary>
/// Cost breakdown per resource
/// </summary>
public class ResourceCostBreakdown
{
    public string Resource { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal HourlyCost { get; set; }
    public decimal MonthlyCost { get; set; }
    public decimal YearlyCost { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Error { get; set; }
}

/// <summary>
/// Azure Retail Prices API response item
/// </summary>
public class AzurePriceItem
{
    public string CurrencyCode { get; set; } = "USD";
    public decimal RetailPrice { get; set; }
    public decimal UnitPrice { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string SkuName { get; set; } = string.Empty;
    public string MeterName { get; set; } = string.Empty;
    public string ArmRegionName { get; set; } = string.Empty;
    public string ArmSkuName { get; set; } = string.Empty;
}

/// <summary>
/// Azure Retail Prices API response
/// </summary>
public class AzurePricesResponse
{
    public List<AzurePriceItem> Items { get; set; } = new();
    public string? NextPageLink { get; set; }
    public int Count { get; set; }
}
