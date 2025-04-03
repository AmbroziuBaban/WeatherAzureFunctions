using DataSync.Constants;
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


        services.AddHttpClient(StringConstants.GetLocationFuncHttpClientName, cfg =>
        {
            var getLocationSyncFuncURL = Environment.GetEnvironmentVariable(StringConstants.GetLocationFuncVariableName);
            if (Uri.TryCreate(getLocationSyncFuncURL, UriKind.Absolute, out var getLocationSyncFuncUri))
            {
                cfg.BaseAddress = getLocationSyncFuncUri;
            }
        });

        services.AddHttpClient(StringConstants.DataExtractorFuncHttpClientName, cfg =>
        {
            var DataExtractorFuncURL = Environment.GetEnvironmentVariable(StringConstants.DataExtractorFuncVariableName);
            if (Uri.TryCreate(DataExtractorFuncURL, UriKind.Absolute, out var DataExtractorFuncURI))
            {
                cfg.BaseAddress = DataExtractorFuncURI;
            }
        });

        services.AddAzureClients(builder =>
        {
            var serviceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");

            builder.AddServiceBusClient(serviceBusConnectionString);
        });
    })
    .Build();

host.Run();
