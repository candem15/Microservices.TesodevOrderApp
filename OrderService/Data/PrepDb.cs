using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Models;
using OrderService.SyncDataServices.Grpc;

namespace OrderService.Data
{
    public class PrepDb
    {
        public static void Migrations(IApplicationBuilder applicationBuilder, bool isProduction)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<ICustomerDataClient>();

                var customers = grpcClient.ReturnAllCustomers();

                ApplyMigrations(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);

                SeedData(serviceScope.ServiceProvider.GetService<IOrderRepo>(), customers);
            }
        }

        private static void ApplyMigrations(AppDbContext dbContext, bool isProduction)
        {
            if (isProduction)
            {
                Console.WriteLine("--> Trying to apply migrations...");

                try
                {
                    dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Migrations could not applied: {ex.Message}");
                }
            }
        }

        private static void SeedData(IOrderRepo repo, IEnumerable<Customer> customers)
        {
            Console.WriteLine("--> Seeding new customers...");

            foreach (var customer in customers)
            {
                if(!repo.ExternalCustomerExists(customer.ExternalID))
                {
                    repo.CreateCustomer(customer);
                }
                repo.SaveChanges();
            }
        }
    }
}