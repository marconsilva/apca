using System.Text.Json;
using AzurePriceCalculatorAgent.McpServer.Models;

namespace AzurePriceCalculatorAgent.McpServer.Services;

/// <summary>
/// Service for getting Azure pricing information
/// </summary>
public class AzurePricingService
{
    private readonly ILogger<AzurePricingService> _logger;
    private readonly HttpClient _httpClient;
    private const string AzureRetailApiBase = "https://prices.azure.com/api/retail/prices";

    public AzurePricingService(ILogger<AzurePricingService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Gets pricing information for specified Azure resources
    /// </summary>
    public async Task<PricingAnalysisResult> GetResourcePricingAsync(List<AzureResource> resources)
    {
        _logger.LogInformation("Fetching pricing for {Count} resources", resources.Count);

        var pricingResults = new List<ResourcePricing>();

        foreach (var resource in resources)
        {
            try
            {
                var pricing = await GetPricingForResourceAsync(resource);
                pricingResults.Add(pricing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pricing for {ServiceName} - {SkuName}", 
                    resource.ServiceName, resource.SkuName);
                
                pricingResults.Add(new ResourcePricing
                {
                    ResourceName = $"{resource.ServiceName} - {resource.SkuName}",
                    ServiceName = resource.ServiceName,
                    SkuName = resource.SkuName,
                    Region = resource.Region,
                    Quantity = resource.Quantity,
                    Error = ex.Message
                });
            }
        }

        return new PricingAnalysisResult
        {
            Status = "success",
            ResourceCount = resources.Count,
            PricingData = pricingResults,
            Instructions = "Use calculate_total_cost to get the total monthly and yearly costs"
        };
    }

    private async Task<ResourcePricing> GetPricingForResourceAsync(AzureResource resource)
    {
        var filters = new List<string>();
        
        if (!string.IsNullOrEmpty(resource.ServiceName))
            filters.Add($"serviceName eq '{resource.ServiceName}'");
        
        if (!string.IsNullOrEmpty(resource.SkuName))
            filters.Add($"armSkuName eq '{resource.SkuName}'");
        
        if (!string.IsNullOrEmpty(resource.Region))
            filters.Add($"armRegionName eq '{resource.Region}'");

        var filterStr = string.Join(" and ", filters);
        var requestUrl = $"{AzureRetailApiBase}?$filter={Uri.EscapeDataString(filterStr)}";

        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var pricesResponse = JsonSerializer.Deserialize<AzurePricesResponse>(content, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (pricesResponse?.Items == null || !pricesResponse.Items.Any())
        {
            return new ResourcePricing
            {
                ResourceName = $"{resource.ServiceName} - {resource.SkuName}",
                ServiceName = resource.ServiceName,
                SkuName = resource.SkuName,
                Region = resource.Region,
                Quantity = resource.Quantity,
                Error = "No pricing data found for this resource"
            };
        }

        var priceItem = pricesResponse.Items.First();
        var hourlyCost = priceItem.RetailPrice;

        return new ResourcePricing
        {
            ResourceName = $"{resource.ServiceName} - {resource.SkuName}",
            ServiceName = resource.ServiceName,
            SkuName = resource.SkuName,
            Region = resource.Region,
            Quantity = resource.Quantity,
            UnitOfMeasure = priceItem.UnitOfMeasure,
            RetailPrice = priceItem.RetailPrice,
            UnitPrice = priceItem.UnitPrice,
            CurrencyCode = priceItem.CurrencyCode,
            HourlyCost = hourlyCost,
            MonthlyCost = hourlyCost * 730 * resource.Quantity, // 730 hours per month average
            YearlyCost = hourlyCost * 8760 * resource.Quantity, // 8760 hours per year
            ProductName = priceItem.ProductName,
            MeterName = priceItem.MeterName
        };
    }

    /// <summary>
    /// Calculates total costs from pricing data
    /// </summary>
    public Task<CostCalculationResult> CalculateTotalCostAsync(List<ResourcePricing> pricingData)
    {
        _logger.LogInformation("Calculating total costs for {Count} resources", pricingData.Count);

        decimal totalHourly = 0;
        decimal totalMonthly = 0;
        decimal totalYearly = 0;
        var currency = "USD";
        var breakdown = new List<ResourceCostBreakdown>();

        foreach (var item in pricingData)
        {
            if (!string.IsNullOrEmpty(item.Error))
            {
                breakdown.Add(new ResourceCostBreakdown
                {
                    Resource = item.ResourceName,
                    Error = item.Error
                });
                continue;
            }

            var itemHourly = item.HourlyCost * item.Quantity;
            var itemMonthly = itemHourly * 730;
            var itemYearly = itemHourly * 8760;

            totalHourly += itemHourly;
            totalMonthly += itemMonthly;
            totalYearly += itemYearly;
            currency = item.CurrencyCode;

            breakdown.Add(new ResourceCostBreakdown
            {
                Resource = item.ResourceName,
                Quantity = item.Quantity,
                HourlyCost = Math.Round(itemHourly, 4),
                MonthlyCost = Math.Round(itemMonthly, 2),
                YearlyCost = Math.Round(itemYearly, 2),
                Currency = currency
            });
        }

        var result = new CostCalculationResult
        {
            Status = "success",
            Currency = currency,
            Summary = new CostSummary
            {
                TotalHourlyCost = Math.Round(totalHourly, 4),
                TotalMonthlyCost = Math.Round(totalMonthly, 2),
                TotalYearlyCost = Math.Round(totalYearly, 2)
            },
            Breakdown = breakdown,
            Notes = new List<string>
            {
                "Monthly costs calculated based on 730 hours per month (average)",
                "Yearly costs calculated based on 8760 hours per year",
                "Costs may vary based on actual usage patterns",
                "Additional costs may apply for data transfer, storage transactions, etc."
            }
        };

        return Task.FromResult(result);
    }
}
