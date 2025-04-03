using Azure.Data.Tables;
using Azure.Messaging.ServiceBus;
using LogInsert.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Net;
using System.Text.Json;

namespace LogInsert
{
    public class LogInsertFunc
    {
        private readonly TableClient _tableClient;

        public LogInsertFunc(TableServiceClient tableServiceClient)
        {
            _tableClient = tableServiceClient.GetTableClient("DataProcessingLog");
            _tableClient.CreateIfNotExists();
        }


        [Function(nameof(LogInsertFunc))]

        public async Task RunAsync([ServiceBusTrigger("weather-data-process", "your_subscription", Connection = "your_connection_string")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {

            var body = JsonSerializer.Deserialize<ContentResult>(message.Body);


            var entry = new DataProcessingLogEntry
            {
                RowKey = message.MessageId,
                ProcessingMessageID = message.MessageId,
                IsLoginSuccessful = body?.StatusCode == (int)HttpStatusCode.OK
            };

            await _tableClient.AddEntityAsync(entry);
        }
    }
}
