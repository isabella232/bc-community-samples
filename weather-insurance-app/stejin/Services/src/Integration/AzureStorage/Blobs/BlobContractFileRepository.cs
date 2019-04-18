using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WeatherInsurance.Integration.AzureStorage.Blobs
{
    public interface IBlobContractFileRepository
    {
        Task<BlobContractFile> GetBlobContractFile(string owner, string fileName, bool includeAbi, bool includeBytecode, bool includeSourceCode);
        Task StoreBlobContractFile(string fileNameWithExtension, Stream fileStream, string owner);
        Task DeleteBlobContractFile(string fileName, string owner, bool deleteJson, bool deleteSol);
    }

    public class BlobContractFileRepository : IBlobContractFileRepository
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudBlobClient _blobClient;
        private readonly CloudBlobContainer _blobContainer;

        public BlobContractFileRepository(CloudStorageAccount storageAccount)
        {
            _storageAccount = storageAccount;
            _blobClient = _storageAccount.CreateCloudBlobClient();
            _blobContainer = _blobClient.GetContainerReference("contracts");
        }

        public async Task<BlobContractFile> GetBlobContractFile(string owner, string fileName, bool includeAbi, bool includeBytecode, bool includeSourceCode)
        {
            var directory = _blobContainer.GetDirectoryReference(owner);

            var result = new BlobContractFile()
            {
                Name = fileName
            };

            if (includeAbi)
            {
                var blob = directory.GetBlockBlobReference($"{fileName}.json");
                if (await blob.ExistsAsync())
                {
                    var content = BlobHelper.ReadJsonFromStream(await blob.OpenReadAsync());
                    result.Abi = content["abi"];
                }
            }

            if (includeBytecode)
            {
                var blob = directory.GetBlockBlobReference($"{fileName}.json");
                if (await blob.ExistsAsync())
                {
                    var content = BlobHelper.ReadJsonFromStream(await blob.OpenReadAsync());
                    result.Bytecode = content["bytecode"].ToString();
                }
            }

            if (includeSourceCode)
            {
                var blob = directory.GetBlockBlobReference($"{fileName}.sol");
                if (await blob.ExistsAsync())
                {
                    var content = BlobHelper.ReadStringFromStream(await blob.OpenReadAsync());
                    result.SourceCode = content;
                }
            }

            return result;
        }

        public async Task StoreBlobContractFile(string fileNameWithExtension, Stream fileStream, string owner)
        {
            var directory = _blobContainer.GetDirectoryReference(owner);
            var blob = directory.GetBlockBlobReference(fileNameWithExtension);
            await blob.UploadFromStreamAsync(fileStream);
        }

        public async Task DeleteBlobContractFile(string fileName, string owner, bool deleteJson, bool deleteSol)
        {
            var directory = _blobContainer.GetDirectoryReference(owner);
            if (deleteJson)
            {
                var blob = directory.GetBlockBlobReference($"{fileName}.json");
                await blob.DeleteAsync();
            }
            if (deleteSol)
            {
                var blob = directory.GetBlockBlobReference($"{fileName}.sol");
                await blob.DeleteAsync();
            }
        }
    }
}
