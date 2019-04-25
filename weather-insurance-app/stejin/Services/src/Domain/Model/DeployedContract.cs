using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WeatherInsurance.Domain.Model
{
    public class DeployedContract
    {

        public string Address { get; set; }

        public ContractFile ContractFile { get; set; }

        public Network Network { get; set; }

        public string Name { get; set; }

        public string OwnerAddress { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ContractType ContractType { get; set; }

        public DateTime ExpirationDateTime { get; set; }

        public string ConstructorArguments { get; set; }

        public string Description { get; set; }

        public bool IsRegistered { get; set; }

        public long GetRequiredFeeAmount()
        {
            var timeSpan = ExpirationDateTime - DateTime.UtcNow;
            return Convert.ToInt64(Math.Ceiling(timeSpan.TotalDays) * 30000000);
        }

    }
}

