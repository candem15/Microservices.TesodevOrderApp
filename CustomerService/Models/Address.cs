using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CustomerService.Models
{
    public class Address
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        public string AddressLine { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public int CityCode { get; set; }
        public Guid CustomerId { get; set; }
        [JsonIgnore]
        public virtual Customer Customer { get; set; }
    }
}