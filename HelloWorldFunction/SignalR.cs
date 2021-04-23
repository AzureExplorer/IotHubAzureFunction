using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

namespace HelloWorldFunction
{
    public static class SignalR
    {
        [FunctionName("SignalR")]
        public static async Task Run(
            [IoTHubTrigger("messages/events", Connection = "IoTHubTriggerConnection")] EventData message,
            [SignalR(HubName = "broadcast")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            var deviceData = JsonConvert.DeserializeObject<DeviceData>(Encoding.UTF8.GetString(message.Body.Array));
            deviceData.DeviceId = Convert.ToString(message.SystemProperties["iothub-connection-device-id"]);

            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("");
            //var content = new StringContent(JsonConvert.SerializeObject(personDetails), System.Text.Encoding.UTF8, "application/json");
            //var result = await client.PostAsync("api/AzureFace/IdentifyPerson", content);
           // result.EnsureSuccessStatusCode();
            log.LogInformation($"C# IoT Hub trigger function processed a message: {JsonConvert.SerializeObject(deviceData)}");
            await signalRMessages.AddAsync(new SignalRMessage()
            {
                Target = "notify",
                Arguments = new[] { JsonConvert.SerializeObject(deviceData) }
            });
        }
    }
}