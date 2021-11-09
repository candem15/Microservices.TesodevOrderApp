using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.Dtos
{
    public class OrderCreateDto
    {
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public Address Addresses { get; set; }
        [Required]
        public Product Products { get; set; }
    }
}