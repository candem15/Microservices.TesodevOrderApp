using System;
using System.Collections.Generic;
using CustomerService.Models;

namespace CustomerService.Dtos
{
    public class CustomerPublishedDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Event { get; set; }
        public string Email { get; set; }
         public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}