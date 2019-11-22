using System;
using Newtonsoft.Json.Linq;

namespace WeatherInsurance.Integration.AzureStorage.Blobs
{

    public class BlobContractFile
    {
        public string Name { get; set; }

        public string ContractType { get; set; }

        public JToken Abi { get; set; }

        public string Bytecode { get; set; }

        public string SourceCode { get; set; }
    }
}

