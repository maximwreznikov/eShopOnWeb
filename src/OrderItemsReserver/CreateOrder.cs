using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OrderItemsReserver
{
    public static class CreateOrder
    {
        [FunctionName("create_order")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, 
            IBinder binder,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            
            var orderId = (int)data.id;
            var outboundBlob = new BlobAttribute($"orders/{orderId}.json", FileAccess.Write);
            await using var writer = binder.Bind<Stream>(outboundBlob);

            var serializedData = JsonConvert.SerializeObject(data);
            var encoder = new UnicodeEncoding();
            await writer.WriteAsync(encoder.GetBytes(serializedData));

            return new OkObjectResult(orderId);
        }
    }
}