using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WeatherInsurance.Integration.AzureStorage.Blobs
{
    public interface IReferenceBlobContractFileRepository
    {
        Task<IEnumerable<BlobContractFile>> GetReferenceContractFiles();
    }

    public class ReferenceBlobContractFileRepository : IReferenceBlobContractFileRepository
    {
        private readonly CloudStorageAccount _storageAccount;
        private IEnumerable<BlobContractFile> _referenceContracts;

        public ReferenceBlobContractFileRepository(CloudStorageAccount storageAccount)
        {
            _storageAccount = storageAccount;
        }

        public async Task<IEnumerable<BlobContractFile>> GetReferenceContractFiles()
        {
            if (_referenceContracts == null)
                _referenceContracts = await GetReferenceContractFilesFromStore();
            return _referenceContracts;
        }

        private async Task<IEnumerable<BlobContractFile>> GetReferenceContractFilesFromStore()
        {
            var result = new List<BlobContractFile>();
            var blobClient = _storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("contracts");
            var directory = container.GetDirectoryReference("reference");
            BlobContinuationToken token = null;
            do
            {
                var blobList = await directory.ListBlobsSegmentedAsync(token);
                foreach (CloudBlob referenceFile in blobList.Results)
                {
                    var fileInfo = new DirectoryInfo(referenceFile.Name.Split('/')[1]);
                    if (fileInfo.Extension == ".json")
                    {
                        var fileName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);

                        if (!fileName.Contains("|"))
                            fileName = $"{fileName}|Default";

                        var referenceContract = BlobHelper.ReadJsonFromStream(await referenceFile.OpenReadAsync());
                        var referenceContractAbi = referenceContract["abi"];
                        var referenceContractBytecode = referenceContract["bytecode"].ToString();
                        var r = new BlobContractFile
                        {
                            Name = fileName,
                            ContractType = fileName.Split('|')[0],
                            Abi = referenceContractAbi,
                            Bytecode = referenceContractBytecode,

                        };

                        var jsonblob = directory.GetBlockBlobReference($"{fileName}.json");
                        if (await jsonblob.ExistsAsync())
                        {
                            var content = BlobHelper.ReadStringFromStream(await jsonblob.OpenReadAsync());
                            r.SourceCode = content;
                        }

                        result.Add(r);
                    }
                }
                token = blobList.ContinuationToken;
            }
            while (token != null);
            return result;
        }
    }
}
