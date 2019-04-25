using System;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace WeatherInsurance.Integration.Blockchain
{
    [Event("InsuranceBought")]
    public class InsuranceBoughtEvent
    {
        [Parameter("address", "user", 1, false)]
        public string User { get; set; }

        [Parameter("uint", "notional", 2, false)]
        public BigInteger Notional { get; set; }

        [Parameter("uint", "premium", 3, false)]
        public BigInteger Premium { get; set; }
    }
}
