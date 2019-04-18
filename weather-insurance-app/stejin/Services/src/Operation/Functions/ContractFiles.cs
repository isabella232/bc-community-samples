using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeatherInsurance.Integration.AzureStorage.Blobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;

namespace WeatherInsurance.Operation.Functions
{
    public class ContractFiles : Controller
    {
        private readonly IBlobContractFileRepository _blobContractFileRepository;

        public ContractFiles() : base()
        {
            var storageAccount = CloudStorageAccount.Parse(_config.GetWebJobsConnectionString("AzureWebJobsStorage"));
            _blobContractFileRepository = new BlobContractFileRepository(storageAccount);
        }

        [FunctionName("Test")]
        public async Task<IActionResult> Test(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            
            var t = _blobContractFileRepository;

            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        [FunctionName("Test2")]
        public IActionResult Test2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            var t = _blobContractFileRepository;

            string name = req.Query["name"];

            log.LogInformation("C# HTTP trigger function processed a request.");

            return !string.IsNullOrEmpty(name)
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }


        [FunctionName("PostContractFile")]
        public async Task<IActionResult> PostContractFile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var authResult = await _auth.HandleAuthenticateAsync(req, log);
                if (!authResult.Succeeded)
                    return new UnauthorizedResult();

                IFormCollection contractFiles = req.Body as IFormCollection; 
                foreach (var file in contractFiles.Files)
                {
                    // Store first - new file will be retrieved in GetContractFile
                    await _blobContractFileRepository.StoreBlobContractFile(file.FileName, file.OpenReadStream(), "WeatherInsurance");

                   // await CheckAndRegisterContractFile("WeatherInsurance", file);

                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            return new OkObjectResult("Complete");
        }


    }
}
