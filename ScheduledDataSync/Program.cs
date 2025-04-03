using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScheduledDataSync;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddHttpClient(nameof(ScheduledDataSyncFunc), cfg =>
        {
            var dataSyncFuncURL = Environment.GetEnvironmentVariable("DataSyncFuncURL");
            if (Uri.TryCreate(dataSyncFuncURL, UriKind.Absolute, out var dataSyncFuncUri))
            {
                cfg.BaseAddress = dataSyncFuncUri;
            }
        });
    })
    .Build();

host.Run();
