using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using System.Text;

namespace WeatherDataInsert
{
    public class WeatherDataInserFunc
    {
        private readonly BlobContainerClient _blobContainerClient;

        public WeatherDataInserFunc(BlobServiceClient blobServiceClient)
        {
            _blobContainerClient = blobServiceClient.GetBlobContainerClient("weather-data");
            _blobContainerClient.CreateIfNotExists();
        }


        [Function(nameof(WeatherDataInserFunc))]
        public async Task Run([QueueTrigger("weather-data-process", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            // string blobName = $"{GetDirectoryNameBasedOMessageInsertTime(message.InsertedOn)}/{message.MessageId}.json";
            string blobName = $"{message.MessageId}.json";
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);

            byte[] byteArray = Encoding.UTF8.GetBytes(message.MessageText);
            Stream myBlob = new MemoryStream(byteArray);
            var blobHeaders = new BlobHttpHeaders
            {
                ContentType = "application/json"
            };

            await blobClient.UploadAsync(myBlob, blobHeaders);
        }

        [Function(nameof(WeatherDataInserFunc))]
        public async Task Run([ServiceBusTrigger("weather-data-process", "your_subscription", Connection = "your_connection_string")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            string blobName = $"{message.MessageId}.json";
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);

            byte[] byteArray = Encoding.UTF8.GetBytes(message.Body.ToString());
            Stream myBlob = new MemoryStream(byteArray);
            var blobHeaders = new BlobHttpHeaders
            {
                ContentType = "application/json"
            };

            await blobClient.UploadAsync(myBlob, blobHeaders);

            await messageActions.CompleteMessageAsync(message);
        }
    }
}
