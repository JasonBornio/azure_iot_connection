using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

// namespace Physio.SQLWrite
// {
//     public class IotTrigger
//     {
//         private readonly ILogger<IotTrigger> _logger;

//         public IotTrigger(ILogger<IotTrigger> logger)
//         {
//             _logger = logger;
//         }

//         [Function("IotTrigger")]
//         public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
//         {
//             _logger.LogInformation("C# HTTP trigger function processed a request.");
//             return new OkObjectResult("Welcome to Azure Functions!");
//         }
//     }
// }


using System;
using System.IO;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace Physio.SQLWrite
{
    public class IotTrigger
    {

        private readonly ILogger<IotTrigger> _logger;

        public IotTrigger(ILogger<IotTrigger> logger)
        {
            _logger = logger;
        }

        [FunctionName("IotTrigger")]
        public static void Run(
            [IoTHubTrigger("messages/events", Connection = "IoTHubConnectionString")] EventData message, ILogger log)
        {
            var messageBody = Encoding.UTF8.GetString(message.Body.Array);

            log.LogInformation($"IoT Hub message received: {messageBody}");

            // Parse the message and write to SQL
            using (SqlConnection conn = new SqlConnection("Your SQL Connection String"))
            {
                conn.Open();
                string query = "INSERT INTO DeviceMessages (message) VALUES (@message)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@message", messageBody);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}