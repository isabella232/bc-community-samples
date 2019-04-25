using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeatherInsurance.Integration.Database;
using WeatherInsurance.Domain.Model;
using WeatherInsurance.Operation.Functions.Authentication;
using WeatherInsurance.Integration.Blockchain;

namespace WeatherInsurance.Operation.Functions
{
    public static class FeeFunctions
    {
        [FunctionName("calculatecontractfee")]
        public static async Task<IActionResult> CalculateContractFee(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "calculatecontractfee/{address}")] HttpRequest req,
            string address,
            ILogger log,
            [Inject(typeof(IBlockchainClientRepository))] IBlockchainClientRepository blockchainRepo,
            [Inject(typeof(IRepository<DeployedContract>))] IRepository<DeployedContract> repo)
        {
            var contract = await repo.Get(address);
            if (contract.ExpirationDateTime < DateTime.UtcNow)
                return new BadRequestObjectResult("Expiration date is in the past");

            var client = blockchainRepo.GetClient(contract.Network.Name);
            var accounts = await client.GetAccounts();

            var fee = new Fee
            {
                Sender = contract.OwnerAddress,
                Recipient = accounts.First(),
                Contract = contract,
                Timestamp = DateTime.UtcNow,
                Amount = contract.GetRequiredFeeAmount()
            };
            return new OkObjectResult(fee);
        }


        [FunctionName("fee")]
        public static async Task<IActionResult> AddFee(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log,
            [Inject(typeof(AsymmetricAuthenticationHandler))] AsymmetricAuthenticationHandler auth,
            [Inject(typeof(IRepository<Fee>))] IRepository<Fee> repo)
        {
            var authResult = await auth.HandleAuthenticateAsync(req, log);
            if (!authResult.Succeeded)
                return new UnauthorizedResult();

            var user = authResult.Principal.Identity.Name;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Fee fee = JsonConvert.DeserializeObject<Fee>(requestBody);

            fee.Timestamp = DateTime.UtcNow;
            fee.IsConfirmed = false;

            var result = await repo.AddNew(fee);

            return new OkObjectResult(result);
        }
    }
}
