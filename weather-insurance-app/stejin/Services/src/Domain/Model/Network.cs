using System;
namespace WeatherInsurance.Domain.Model
{
    public class Network
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public Platform Platform { get; set; }

        public Uri Url { get; set; }

        public string ReferenceContractAddress { get; set; }
    }
}
