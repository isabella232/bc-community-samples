using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WeatherInsurance.Domain.Model
{
    public class ContractFile
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string CompiledFileName { get { return $"{Name}.json"; } }

        public string SourceFileName { get { return $"{Name}.sol"; } }

        public string OwnerAddress { get; set; }

        public string Description { get; set; }

        public string ApiType { get; set; }

        public string ApiVersion { get; set; }

        public Newtonsoft.Json.Linq.JToken Abi { get; set; }

        public string Bytecode { get; set; }

        public string SourceCode { get; set; }

        public bool IncludesJson { get; set; }

        public bool IncludesSol { get; set; }

        public bool IsComplete { get { return IncludesJson && IncludesSol; } }
    }
}