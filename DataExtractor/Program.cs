using DataExtractor;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddHttpClient(nameof(DataExtractorFunc), cfg =>
        {
            var dataSourceURL = Environment.GetEnvironmentVariable("DataSourceURL");
            var apiKey = Environment.GetEnvironmentVariable("OpenWeatherAPIKey");

            if (Uri.TryCreate(dataSourceURL, UriKind.Absolute, out var dataSourceUri))
            {
                UriBuilder uriBuilder = new UriBuilder(dataSourceUri);
                uriBuilder.Query = $"appId={apiKey}";

                cfg.BaseAddress = uriBuilder.Uri;               
            }
        });
    })
    .Build();

host.Run();
