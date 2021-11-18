using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OrderService.Models
{
    public class Customer
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid ExternalID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [JsonIgnore]
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        [JsonIgnore]
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}