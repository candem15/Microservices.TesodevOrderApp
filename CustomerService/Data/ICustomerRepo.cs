using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.Models;

namespace CustomerService.Data
{
    public interface ICustomerRepo
    {
        bool SaveChanges();
        IEnumerable<Customer> GetAllCustomers();
        Customer GetCustomerById(Guid id);
        /*IEnumerable<Address> GetAdressesByCustomerId(Guid id);""Test Purposes"" */
        bool CreateCustomer(Customer customer);
        bool UpdateCustomer(Customer customer,Address? address);
        bool DeleteCustomer(Guid id);
        bool ValidateCustomer(Guid id);


    }
}