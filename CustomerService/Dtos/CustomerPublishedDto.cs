using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.Models;

namespace CustomerService.Dtos
{
    public class CustomerPublishedDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Event { get; set; }
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}