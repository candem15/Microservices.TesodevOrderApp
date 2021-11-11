using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
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
        public Customer Customer { get; set; }
        public ICollection<Order> Order { get; set; } = new List<Order>();
    }
}