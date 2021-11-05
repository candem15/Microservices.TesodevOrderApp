using CustomerService.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {

        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Address>()
                .HasOne(p => p.Customer)
                .WithMany(b => b.Addresses);
            modelBuilder
                .Entity<Customer>()
                .HasMany(p => p.Addresses)
                .WithOne(b => b.Customer);
        }
    }
}