using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace WeatherInsurance.Integration.Database.Model
{
    public class Purchase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PurchaseId { get; set; }

        [MinLength(42)]
        [MaxLength(42)]
        public string ContractAddress { get; set; }

        [Required]
        public long NetworkId { get; set; }

        public DeployedContract Contract { get; set; }

        [MinLength(42)]
        [MaxLength(42)]
        public string UserAddress { get; set; }

        [Required]
        public decimal Notional { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 6)")]
        public decimal Premium { get; set; }

        [Required]
        public DateTime PurchaseTimestamp { get; set; }
    }
}