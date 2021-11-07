using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.Models;

namespace CustomerService.Dtos
{
    public class CustomerUpdateDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Address Addresses { get; set; }
    }
}