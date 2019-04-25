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
    public static class PurchaseFunctions
    {

        [FunctionName("purchases")]
        public static async Task<IActionResult> GetPurchases(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "purchases/{contractAddress}")] HttpRequest req,
                string contractAddress,
                ILogger log,
                [Inject(typeof(IRepository<Purchase>))] IRepository<Purchase> repo)
        {
            var contracts = await repo.Find(d => d.Contract.Address == contractAddress);
            return new OkObjectResult(contracts);
        }

    }
}
