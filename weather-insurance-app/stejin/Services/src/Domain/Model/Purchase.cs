using System;
using System.Numerics;

namespace WeatherInsurance.Domain.Model
{
    public class Purchase
    {
        public long Id { get; set; }

        public DeployedContract Contract { get; set; }

        public DateTime Timestamp { get; set; }

        public string UserAddress { get; set; }

        public decimal Notional { get; set; }

        public decimal Premium { get; set; }

    }
}
