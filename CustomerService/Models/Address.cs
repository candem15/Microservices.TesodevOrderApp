using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models
{
    public class Address
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid CustomerId { get; set; }
        public string AddressLine { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public int CityCode { get; set; }
        public Customer Customer { get; set; }
    }
}