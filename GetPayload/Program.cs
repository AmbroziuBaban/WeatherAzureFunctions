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
            var blobServiceConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            builder.AddBlobServiceClient(blobServiceConnectionString);
        });
    })
    .Build();

host.Run();

