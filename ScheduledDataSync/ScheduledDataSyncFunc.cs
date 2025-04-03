using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ScheduledDataSync
{
    public class ScheduledDataSyncFunc
    {
        private readonly HttpClient _dataSyncFuncHttpClient;

        public ScheduledDataSyncFunc(IHttpClientFactory httpClientFactory)
        {
            _dataSyncFuncHttpClient = httpClientFactory.CreateClient(nameof(ScheduledDataSyncFunc));
        }

        [Function(nameof(ScheduledDataSyncFunc))]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            await _dataSyncFuncHttpClient.GetAsync(_dataSyncFuncHttpClient.BaseAddress);
        }
    }
}
