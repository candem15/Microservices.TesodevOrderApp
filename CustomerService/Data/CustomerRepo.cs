using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CustomerService.Models;

namespace CustomerService.Data
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly AppDbContext _context;

        public CustomerRepo(AppDbContext context)
        {
            _context = context;
        }
        public bool CreateCustomer(Customer customer)
        {
            if (customer == null)
            {
                string nullException = new ArgumentNullException(nameof(customer)).ToString();
                Console.WriteLine($"--> Customer could not created : {nullException}");
                return false;
            }
            _context.Customers.Add(customer);
            SaveChanges();
            Console.WriteLine("--> New Customer created successfully!");
            return true;
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
            return _context.Customers.ToList();
        }

        public Customer GetCustomerById(Guid id)
        {
            return _context.Customers.FirstOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0); //This will apply changes even slightly different from previous version of itself.
        }

        public bool UpdateCustomer(Customer customer)
        {
            if (!ValidateCustomer(customer.Id))
            {
                return false;
            }
            _context.Customers.Update(customer);
            SaveChanges();
            Console.WriteLine("--> Customer updated successfully!");
            return true;
        }

        public bool ValidateCustomer(Guid id)
        {
            var customer = _context.Customers.FirstOrDefault(p => p.Id == id);
            if(customer!=null)
            {
                return true;
            }
            return false;
        }
    }
}