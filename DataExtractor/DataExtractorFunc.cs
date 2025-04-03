using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Web;

namespace DataExtractor
{
    public class DataExtractorFunc
    {
        private readonly HttpClient _httpClient;

        public DataExtractorFunc(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient(nameof(DataExtractorFunc));
        }

        [Function(nameof(DataExtractorFunc))]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            var location = req.Query["location"].FirstOrDefault();


            UriBuilder uriBuilder = new UriBuilder(_httpClient.BaseAddress);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            var appid = query["appid"];
            query.Remove("appid");


            query["q"] = location;
            query["appid"] = appid;

            uriBuilder.Query = query.ToString();

            HttpResponseMessage response = await _httpClient.GetAsync(uriBuilder.Uri);

            string content = await response.Content.ReadAsStringAsync();
            return new ContentResult
            {
                Content = content,
                ContentType = "application/json",
                StatusCode = (int)response.StatusCode
            };
        }
    }
}
