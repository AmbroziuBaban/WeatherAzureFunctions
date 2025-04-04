using DataSync.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace DataSync
{
    public class DataSyncFunc
    {
        private readonly HttpClient _getLocationFuncHttpClient;
        private readonly HttpClient _dataExtractorFuncHttpClient;

        public DataSyncFunc(IHttpClientFactory httpClientFactory)
        {
            _getLocationFuncHttpClient = httpClientFactory.CreateClient(StringConstants.GetLocationFuncHttpClientName);
            _dataExtractorFuncHttpClient = httpClientFactory.CreateClient(StringConstants.DataExtractorFuncHttpClientName);
        }

        [Function(nameof(DataSyncFunc))]
        [ServiceBusOutput("weather-data-process", Connection = "your_connection_string")]
        public async Task<string[]> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req,
            ExecutionContext executionContext)
        {
            var getLocationsResponse = await _getLocationFuncHttpClient.GetAsync(_getLocationFuncHttpClient.BaseAddress);
            string[] locations = await getLocationsResponse.Content.ReadFromJsonAsync<string[]>() ?? [];

            List<string> messages = new List<string>();

            foreach (string location in locations)
            {
                var address = $"{_dataExtractorFuncHttpClient.BaseAddress}?location={location}";

                var extractedDataResponse = await _dataExtractorFuncHttpClient.GetAsync(address);

                Guid syncID = Guid.NewGuid();

                var responseContentStream = await extractedDataResponse.Content.ReadAsStreamAsync();
                StreamReader readStream = new StreamReader(responseContentStream, Encoding.UTF8);
                var responseContent = readStream.ReadToEnd();
                var result = new ContentResult
                {
                    Content = responseContent,
                    StatusCode = (int)extractedDataResponse.StatusCode
                };

                messages.Add(JsonSerializer.Serialize(result));
            }

            return messages.ToArray();
        }
    }
}
