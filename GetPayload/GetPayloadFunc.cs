using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace GetPayload
{
    public class GetPayloadFunc
    {
        private readonly BlobContainerClient _blobContainerClient;

        public GetPayloadFunc(BlobServiceClient blobServiceClient)
        {
            _blobContainerClient = blobServiceClient.GetBlobContainerClient("weather-data");
        }

        [Function(nameof(GetPayloadFunc))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            var logEntryIdQueryParam = req.Query["logEntryID"].FirstOrDefault();
            var blobName = $"{logEntryIdQueryParam}.json";

            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            if (!await blobClient.ExistsAsync())
            {
                return new NotFoundObjectResult(string.Empty);
            }

            var blob = await blobClient.DownloadToAsync(blobName);

            return new OkObjectResult(blob);
        }
    }
}
