using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();


        services.AddAzureClients(builder =>
        {
            var tableServiceConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            builder.AddTableServiceClient(tableServiceConnectionString);
        });
    })
    .Build();

host.Run();
