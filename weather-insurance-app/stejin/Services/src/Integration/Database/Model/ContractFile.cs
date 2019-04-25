using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherInsurance.Integration.Database.Model
{
    public class ContractFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ContractFileId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ContractFileName { get; set; }

        [Required]
        public bool IncludesJson { get; set; }

        [Required]
        public bool IncludesSol { get; set; }

        [MinLength(42)]
        [MaxLength(42)]
        [Required]
        public string OwnerAddress { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(25)]
        public string ApiType { get; set; }

        [MaxLength(25)]
        public string ApiVersion { get; set; }
    }
}
