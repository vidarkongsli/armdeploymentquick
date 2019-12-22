using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Antiboilerplate.Functional;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace ARMState
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ResourceGroupState
    {
        [JsonProperty("value")]
        public string CurrentValue { get; set; }

        public bool IsEqualTo(string value) => ComputeSha256Hash(value) == CurrentValue;

        public string Set(string value) => ComputeSha256Hash(value).Then(v => CurrentValue = v);

        [FunctionName(nameof(ResourceGroupState))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<ResourceGroupState>();

        private static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using var sha256Hash = SHA256.Create();
            // ComputeHash - returns byte array  
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string   
            var builder = new StringBuilder();
            foreach (var t in bytes)
            {
                builder.Append(t.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
