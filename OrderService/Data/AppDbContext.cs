using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {

        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Order>()
                .HasOne(p => p.Addresses)
                .WithMany(b => b.Order);
            modelBuilder
                .Entity<Address>()
                .HasMany(p => p.Order)
                .WithOne(b => b.Addresses);
            modelBuilder
                .Entity<Customer>()
                .HasMany(p => p.Orders)
                .WithOne(b => b.Customer)
                .HasForeignKey(p=>p.CustomerId);
            modelBuilder
                .Entity<Order>()
                .HasOne(p => p.Customer)
                .WithMany(b => b.Orders)
                .HasForeignKey(p=>p.CustomerId);
            modelBuilder
                .Entity<Order>()
                .HasOne(p => p.Products)
                .WithMany(b => b.Order);
            modelBuilder
                .Entity<Product>()
                .HasMany(p => p.Order)
                .WithOne(b => b.Products);
        }
    }
}