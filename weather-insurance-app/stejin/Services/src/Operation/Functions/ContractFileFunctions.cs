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
using WeatherInsurance.Operation.Functions.Authentication;
using WeatherInsurance.Integration.Database;
using WeatherInsurance.Domain.Model;
using System.Linq;
using WeatherInsurance.Integration.Blockchain;
using System.Collections.Generic;

namespace WeatherInsurance.Operation.Functions
{
    public static class ContractFileFunctions
    {


        [FunctionName("contractfiles")]
        public static async Task<IActionResult> GetContractFiles(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject(typeof(AsymmetricAuthenticationHandler))] AsymmetricAuthenticationHandler auth,
            [Inject(typeof(IBlobContractFileRepository))] IBlobContractFileRepository blobContractFileRepository,
            [Inject(typeof(IRepository<ContractFile>))] IRepository<ContractFile> repo)
        {
            var authResult = await auth.HandleAuthenticateAsync(req, log);
            if (!authResult.Succeeded)
                return new UnauthorizedResult();

            var user = authResult.Principal.Identity.Name;

            var result = await repo.Find(c => c.OwnerAddress == user);
            return new OkObjectResult(result);
        }

        [FunctionName("contractfile")]
        public static async Task<IActionResult> CreateUpdateDeleteContractFile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = null)] HttpRequest req,
            ILogger log,
            [Inject(typeof(AsymmetricAuthenticationHandler))] AsymmetricAuthenticationHandler auth,
            [Inject(typeof(IBlobContractFileRepository))] IBlobContractFileRepository blobContractFileRepository,
            [Inject(typeof(IReferenceBlobContractFileRepository))] IReferenceBlobContractFileRepository referenceBlobContractFileRepository,
            [Inject(typeof(IRepository<ContractFile>))] IRepository<ContractFile> repo)
        {
            if (req.Method.ToUpper() == "GET")
            {
                string id = req.Query["id"];
                long userId;

                if (long.TryParse(id, out userId))
                {
                    var result = await repo.Get(userId);

                    if (req.Query.ContainsKey("include"))
                    {
                        var includedItems = req.Query["include"].ToString().Split(',');

                        var blobContractFile = await blobContractFileRepository.GetBlobContractFile(result.OwnerAddress, result.Name,
                                includedItems.Contains("abi", StringComparer.InvariantCultureIgnoreCase),
                                includedItems.Contains("bytecode", StringComparer.InvariantCultureIgnoreCase),
                                includedItems.Contains("sourcecode", StringComparer.InvariantCultureIgnoreCase) || includedItems.Contains("source", StringComparer.InvariantCultureIgnoreCase));

                        result.Abi = blobContractFile.Abi;
                        result.Bytecode = blobContractFile.Bytecode;
                        result.SourceCode = blobContractFile.SourceCode;
                    }
                    return new OkObjectResult(result);
                } else
                {
                    return new BadRequestObjectResult("Please include parameter user");
                }
            }
            else
            {
                var authResult = await auth.HandleAuthenticateAsync(req, log);
                if (!authResult.Succeeded)
                    return new UnauthorizedResult();

                var user = authResult.Principal.Identity.Name;

                switch (req.Method.ToUpper())
                {
                    case "POST": return await CreateContractFile(user, req, log, repo, blobContractFileRepository, referenceBlobContractFileRepository);
                    case "PUT": return await UpdateContractFile(user, req, repo);
                    case "DELETE": return await DeleteContractFile(user, req, repo, blobContractFileRepository);
                    default: return new BadRequestObjectResult($"Unexpected operation: {req.Method}");
                }
            }
        }

        private static async Task<IActionResult> CreateContractFile(string user, HttpRequest req, ILogger log, IRepository<ContractFile> repository, IBlobContractFileRepository blobContractFileRepository, IReferenceBlobContractFileRepository referenceBlobContractFileRepository)
        {
            try
            {
                IFormCollection contractFiles = req.Form as IFormCollection;
                foreach (var file in contractFiles.Files)
                {
                    // Store first - new file will be retrieved in GetContractFile
                    await blobContractFileRepository.StoreBlobContractFile(file.FileName, file.OpenReadStream(), user);

                    await CheckAndRegisterContractFile(repository, blobContractFileRepository, referenceBlobContractFileRepository, user, file);

                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            return new OkObjectResult("Complete");
        }

        private static async Task CheckAndRegisterContractFile(IRepository<ContractFile> repository, IBlobContractFileRepository blobContractFileRepository, IReferenceBlobContractFileRepository referenceBlobContractFileRepository, string ownerAddress, IFormFile file)
        {
            var fileInfo = new DirectoryInfo(file.FileName);
            var fileName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);

            var contractFile = (await repository.Find(c => c.OwnerAddress == ownerAddress && c.Name == fileName)).FirstOrDefault()
                                ?? new ContractFile()
                                {
                                    Name = fileName,
                                    OwnerAddress = ownerAddress,
                                };

            if (fileInfo.Extension.ToLower() == ".json")
            {
                if (contractFile.IncludesJson)
                    throw new IOException($"{fileInfo.Name} already exists");

                contractFile.IncludesJson = true;

                var blobContractFile = await blobContractFileRepository.GetBlobContractFile(ownerAddress, fileName, true, false, false);

                var targetContractCode = blobContractFile.Abi;
                var referenceContracts = (await referenceBlobContractFileRepository.GetReferenceContractFiles()).OrderBy(f => f.Name).Reverse();
                foreach (var r in referenceContracts)
                {
                    var f = r.Name;
                    var contractComparer = new ContractComparer(r.Abi.ToString(), targetContractCode.ToString());
                    if (contractComparer.IsInterfaceImplemented())
                    {
                        var contractType = f.Split('|')[0];
                        var version = f.Split('|')[1];
                        contractFile.ApiType = contractType;
                        contractFile.ApiVersion = version;
                    }
                }
            }

            if (fileInfo.Extension.ToLower() == ".sol")
            {
                if (contractFile.IncludesSol)
                    throw new IOException($"{fileInfo.Name} already exists");

                contractFile.IncludesSol = true;
            }

            if (contractFile.Id == 0)
                await repository.AddNew(contractFile);
            else
                await repository.Update(contractFile);
        }

        private static async Task<IActionResult> UpdateContractFile(string user, HttpRequest req, IRepository<ContractFile> repository)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                ContractFile contractFile = JsonConvert.DeserializeObject<ContractFile>(requestBody);

                var existingContractFile = await repository.Get(contractFile.Id);
                if (existingContractFile.OwnerAddress != user)
                    return new UnauthorizedResult();

                await repository.Update(contractFile);
                return new OkObjectResult("Complete");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        private static async Task<IActionResult> DeleteContractFile(string user, HttpRequest req, IRepository<ContractFile> repository, IBlobContractFileRepository blobContractFileRepository)
        {
            try
            {
                var id = long.Parse(req.Query["id"]);

                var contractFile = await repository.Get(id);
                if (contractFile.OwnerAddress != user)
                    return new UnauthorizedResult();

                await blobContractFileRepository.DeleteBlobContractFile(contractFile.Name, user, contractFile.IncludesJson, contractFile.IncludesSol);
                await repository.Delete(id);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            return new OkObjectResult("Complete");
        }

        [FunctionName("referencecontractfile")]
        public static async Task<IActionResult> CalculateContractFee(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "contractfiles/reference/{contractType}")] HttpRequest req,
            string contractType,
            ILogger log,
            [Inject(typeof(IReferenceBlobContractFileRepository))] IReferenceBlobContractFileRepository referenceBlobContractFileRepository)
        {
            var referenceContractFiles = await referenceBlobContractFileRepository.GetReferenceContractFiles();
            BlobContractFile blobContractFile = referenceContractFiles.Where(r => r.ContractType == contractType).OrderBy(f => f.Name).Reverse().First();
            return new OkObjectResult(blobContractFile);
        }

    }
}
