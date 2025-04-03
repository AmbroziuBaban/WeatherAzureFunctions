using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace GetLocations
{
    public class GetLocationsFunc
    {

        [Function(nameof(GetLocationsFunc))]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            string[] locations = ["London"];

            return new OkObjectResult(locations);
        }
    }
}
