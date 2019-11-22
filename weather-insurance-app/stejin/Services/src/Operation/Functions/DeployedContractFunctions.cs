using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeatherInsurance.Operation.Functions.Authentication;
using WeatherInsurance.Integration.Database;
using WeatherInsurance.Domain.Model;
using System.Linq;
using Nethereum.Hex.HexConvertors.Extensions;

namespace WeatherInsurance.Operation.Functions
{
    public static class DeployedContractFunctions
    {
        [FunctionName("deployedcontracts")]
        public static async Task<IActionResult> GetDeployedContracts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject(typeof(AsymmetricAuthenticationHandler))] AsymmetricAuthenticationHandler auth,
            [Inject(typeof(IRepository<DeployedContract>))] IRepository<DeployedContract> repo)
        {
            var authResult = await auth.HandleAuthenticateAsync(req, log);
            if (!authResult.Succeeded)
                return new UnauthorizedResult();

            var user = authResult.Principal.Identity.Name;

            var result = await repo.Find(c => c.OwnerAddress == user);

            if (req.Query.ContainsKey("id"))
            {
                result = result.Where(c => c.ContractFile.Id == long.Parse(req.Query["id"]));
            }

            if (req.Query.ContainsKey("fileid"))
            {
                result = result.Where(c => c.ContractFile.Id == long.Parse(req.Query["fileid"]));
            }

            return new OkObjectResult(result);
        }

        [FunctionName("deployedcontract")]
        public static async Task<IActionResult> RegisterDeployedContracts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log,
            [Inject(typeof(AsymmetricAuthenticationHandler))] AsymmetricAuthenticationHandler auth,
            [Inject(typeof(IRepository<DeployedContract>))] IRepository<DeployedContract> repo)
        {
            var authResult = await auth.HandleAuthenticateAsync(req, log);
            if (!authResult.Succeeded)
                return new UnauthorizedResult();

            var user = authResult.Principal.Identity.Name;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            DeployedContract contract = JsonConvert.DeserializeObject<DeployedContract>(requestBody);

            // Reset owner to address of authenticated user
            contract.OwnerAddress = user;

            var result = await repo.AddNew(contract);

            return new OkObjectResult(result);
        }

        [FunctionName("validatecontractname")]
        public static async Task<IActionResult> ValidateContractName(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "validatecontractname/{networkId}")] HttpRequest req,
            long networkId,
            ILogger log,
            [Inject(typeof(IRepository<DeployedContract>))] IRepository<DeployedContract> repo)
        {

            string name = req.Query["name"];

            if (string.IsNullOrEmpty(name))
                return new BadRequestObjectResult("Please specify name parameter");

            var contracts = await repo.Find(d => d.Network.Id == networkId && d.Name == name.HexToUTF8String());
            if (contracts.Any())
            {
                return new OkObjectResult("Name already in use.");
            }
            return new OkObjectResult(string.Empty);
        }

        [FunctionName("registeredcontracts")]
        public static async Task<IActionResult> GetRegisteredContracts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "registeredcontracts/{networkId}")] HttpRequest req,
            long networkId,
            ILogger log,
            [Inject(typeof(IRepository<DeployedContract>))] IRepository<DeployedContract> repo)
        {
            var result = await repo.Find(c => c.Network.Id == networkId && c.IsRegistered);

            if (req.Query.ContainsKey("apitype"))
            {
                result = result.Where(c => c.ContractFile.ApiType == req.Query["apitype"]);
            }

            if (req.Query.ContainsKey("isactive") && bool.Parse(req.Query["isactive"]))
            {
                result = result.Where(c => c.ExpirationDateTime >= DateTime.UtcNow);
            }

            if (req.Query.ContainsKey("isactive") && !bool.Parse(req.Query["isactive"]))
            {
                result = result.Where(c => c.ExpirationDateTime < DateTime.UtcNow);
            }

            return new OkObjectResult(result);
        }
    }
}
