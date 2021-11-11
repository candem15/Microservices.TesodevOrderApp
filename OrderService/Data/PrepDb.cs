using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Data
{
    public class PrepDb
    {
        public static void Migrations(IApplicationBuilder applicationBuilder, bool isProduction)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                ApplyMigrations(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
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
    }
}