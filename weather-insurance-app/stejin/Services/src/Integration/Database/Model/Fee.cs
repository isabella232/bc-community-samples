using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherInsurance.Integration.Database.Model
{
    public class Fee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long FeeId { get; set; }

        [MinLength(42)]
        [MaxLength(42)]
        [Required]
        public string Sender { get; set; }

        [MinLength(42)]
        [MaxLength(42)]
        [Required]
        public string Recipient { get; set; }

        [Required]
        public string ContractAddress { get; set; }

        [Required]
        public long NetworkId { get; set; }

        public DeployedContract Contract { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public long Amount { get; set; }

        [MinLength(66)]
        [MaxLength(66)]
        [Required]
        public string TransactionHash { get; set; }


        [Required]
        public bool IsConfirmed { get; set; }

    }
}
