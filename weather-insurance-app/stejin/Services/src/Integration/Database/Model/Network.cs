using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherInsurance.Integration.Database.Model
{
    public class Network
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long NetworkId { get; set; }

        [Required]
        [MaxLength(50)]
        public string NetworkName { get; set; }

        [Required]
        public int Platform { get; set; }

        [Required]
        [MaxLength(255)]
        public string Url { get; set; }

        [MinLength(42)]
        [MaxLength(42)]
        [Required]
        public string ReferenceContractAddress { get; set; }

    }
}
