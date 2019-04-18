using System;
using System.ComponentModel.DataAnnotations;

namespace WeatherInsurance.Integration.Database.Model
{
    public class DeployedContract
    {

        [MinLength(42)]
        [MaxLength(42)]
        public string ContractAddress { get; set; }

        [Required]
        public long ContractFileId { get; set; }

        public ContractFile ContractFile { get; set; }

        [Required]
        public long NetworkId { get; set; }

        public Network Network { get; set; }

        [Required]
        [MaxLength(20)]
        public string ContractName { get; set; }

        [MinLength(42)]
        [MaxLength(42)]
        [Required]
        public string OwnerAddress { get; set; }

        [Required]
        public int ContractType { get; set; }

        public DateTime ExpirationDateTime { get; set; }

        public string ConstructorArguments { get; set; }

    }
}
