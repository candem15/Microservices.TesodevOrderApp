using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CustomerService.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Data
{
    public class CustomerRepo<T> : ICustomerRepo<T> where T : class
    {
        private readonly AppDbContext _context;

        public CustomerRepo(AppDbContext context)
        {
            _context = context;
        }
        public Guid CreateCustomer(Customer customer)
        {
            if (customer == null)
            {
                string nullException = new ArgumentNullException(nameof(customer)).ToString();
                Console.WriteLine($"--> Customer could not created : {nullException}");
                return Guid.Empty;
            }
            _context.Customers.Add(customer);
            _context.Addresses.AddRange(customer.Addresses);
            SaveChanges();
            return customer.Id;
        }

        public bool DeleteCustomer(Guid id)
        {
            if (!ValidateCustomer(id))
            {
                Console.WriteLine($"--> Customer could not deleted!");
                return false;
            }
            _context.Customers.Remove(GetCustomerById(id));
            SaveChanges();
            Console.WriteLine("--> Customer deleted successfully!");
            return true;
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            var result = (from cust in _context.Customers.ToList()
                          join adr in _context.Addresses.ToList()
                          on cust.Id equals adr.CustomerId
                          select cust).Distinct().ToList();

            return result;
        }

        public Customer GetCustomerById(Guid id)
        {
            var result = (from cust in _context.Customers.ToList()
                          join adr in _context.Addresses.ToList()
                          on cust.Id equals adr.CustomerId
                          select cust).FirstOrDefault(p => p.Id == id);

            return result;
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0); //This will apply changes even slightly different from previous version of itself.
        }

        public bool UpdateCustomer(Customer customer, Address? address)
        {
            if (!ValidateCustomer(customer.Id))
            {
                return false;
            }
            ICollection<Address> newAddress = new List<Address>();
            newAddress = customer.Addresses;
            newAddress.Remove(_context.Addresses.FirstOrDefault(p=>p.Id==address.Id));
            newAddress.Add(address);
            customer.Addresses=newAddress;
            _context.Customers.Update(customer);
            SaveChanges();
            Console.WriteLine("--> Customer updated successfully!");
            return true;
        }

        public bool ValidateCustomer(Guid id)
        {
            return _context.Customers.Any(p => p.Id == id);
        }
    }
}