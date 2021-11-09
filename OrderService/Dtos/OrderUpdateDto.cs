using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.Dtos
{
    public class OrderUpdateDto
    {
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }
        public Address Addresses { get; set; }
        public Product Products { get; set; }
    }
}