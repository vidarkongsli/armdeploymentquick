using System.IO;
using System.Threading.Tasks;
using Antiboilerplate.String;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
// ReSharper disable UnusedMember.Global

namespace ARMState
{
    public static class CheckState
    {
        [FunctionName("CheckState")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            [DurableClient] IDurableEntityClient client, ILogger log)
        {
            string key = req.Query["key"];
            if (key.IsNullOrWhitespace())
                return new BadRequestObjectResult("Missing query parameter: key");

            var entityId = new EntityId(nameof(ResourceGroupState), key);
            var entity = await client.ReadEntityStateAsync<ResourceGroupState>(entityId);

            if (!entity.EntityExists)
            {
                return CreateStatusResponse("changed");
            }

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            return CreateStatusResponse(entity.EntityState.IsEqualTo(requestBody) ? "unchanged" : "changed");
        }

        private static IActionResult CreateStatusResponse(string state)
            => new ContentResult
            {
                Content = $@"{{""state"":""{state}""}}",
                StatusCode = StatusCodes.Status200OK,
                ContentType = System.Net.Mime.MediaTypeNames.Application.Json
            };
    }
}
