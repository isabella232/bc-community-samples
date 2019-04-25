using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeatherInsurance.Integration.Database;
using WeatherInsurance.Domain.Model;

namespace WeatherInsurance.Operation.Functions
{
    public static class NetworkFunctions
    {
        [FunctionName("networks")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject(typeof(IRepository<Network>))] IRepository<Network> repo)
        {
            var networks = await repo.Get();

            return new OkObjectResult(networks);
        }
    }
}
