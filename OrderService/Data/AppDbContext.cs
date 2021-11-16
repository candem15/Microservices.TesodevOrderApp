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
                .Entity<Address>()
                .HasMany<Order>(p => p.Order)
                .WithOne(b => b.Address)
                .HasForeignKey(p=>p.AddressId);
            modelBuilder
                .Entity<Customer>()
                .HasMany<Order>(p => p.Orders)
                .WithOne(b => b.Customer)
                .HasForeignKey(p=>p.CustomerId);
            modelBuilder
                .Entity<Product>()
                .HasMany<Order>(p => p.Orders)
                .WithOne(b => b.Product)
                .HasForeignKey(p=>p.ProductId);
            modelBuilder
                .Entity<Address>()
                .HasOne<Customer>(p => p.Customer)
                .WithMany(b => b.Addresses)
                .OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey(p=>p.CustomerId);
        }
    }
}