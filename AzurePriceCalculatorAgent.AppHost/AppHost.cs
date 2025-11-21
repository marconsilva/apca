var builder = DistributedApplication.CreateBuilder(args);

var mcpServer = builder.AddProject<Projects.AzurePriceCalculatorAgent_McpServer>("mcpserver")
    .WithHttpHealthCheck("/health");

var apiService = builder.AddProject<Projects.AzurePriceCalculatorAgent_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.AzurePriceCalculatorAgent_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WithReference(mcpServer)
    .WaitFor(apiService)
    .WaitFor(mcpServer);

builder.Build().Run();
