using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.Models;

namespace CustomerService.Dtos
{
    public class CustomerCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}