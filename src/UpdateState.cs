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
    public static class UpdateState
    {
        [FunctionName("UpdateState")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableEntityClient client, ILogger log)
        {
            string key = req.Query["key"];
            if (key.IsNullOrWhitespace())
                return new BadRequestObjectResult("Missing query parameter: key");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var entityId = new EntityId(nameof(ResourceGroupState), key);
            await client.SignalEntityAsync(entityId, "Set", requestBody);
            
            return new OkResult();
        }
    }
}
