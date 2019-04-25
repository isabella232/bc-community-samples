using System;
namespace WeatherInsurance.Domain.Model
{
    public class Fee
    {
        public long Id { get; set; }

        public string Sender { get; set; }

        public string Recipient { get; set; }

        public DeployedContract Contract { get; set; }

        public DateTime Timestamp { get; set; }

        public long Amount { get; set; }

        public string TransactionHash { get; set; }

        public bool IsConfirmed { get; set; }
    }


}
