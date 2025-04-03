using Azure.Data.Tables;
using GetLogsBetweenDates.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace GetLogsBetweenDates
{
    public class GetLogsBetweenDatesFunc
    {
        private readonly TableClient _tableClient;

        public GetLogsBetweenDatesFunc(TableServiceClient tableServiceClient)
        {
            _tableClient = tableServiceClient.GetTableClient("DataProcessingLog");
        }

        [Function(nameof(GetLogsBetweenDatesFunc))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            var fromDateQueryParam = req.Query["from"].FirstOrDefault();
            var toDateQueryParam = req.Query["to"].FirstOrDefault();

            List<DataProcessingLogEntry> entries = new List<DataProcessingLogEntry>();

            if (DateTime.TryParse(fromDateQueryParam, out DateTime fromDateTime) && DateTime.TryParse(toDateQueryParam, out DateTime toDateTime))
            {
                var filter = TableClient.CreateQueryFilter($"Timestamp ge {fromDateTime:O} and Timestamp le {toDateTime:o}");
                await foreach (var entry in _tableClient.QueryAsync<DataProcessingLogEntry>(filter))
                {
                    entries.Add(entry);
                }

                return new OkObjectResult(entries);
            }
            else
            {
                return new BadRequestObjectResult(string.Empty);
            }
        }
    }
}
