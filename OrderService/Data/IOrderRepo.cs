using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.Data
{
    public interface IOrderRepo
    {
        bool SaveChanges();
        IEnumerable<Order> GetAllOrders();
        IEnumerable<Order> GetAllOrdersByCustomerId(Guid id);
        Order GetOrderById(Guid id);
        Guid CreateOrder(Order order);
        void CreateCustomer(Customer customer);
        bool UpdateOrder(Order order);
        bool DeleteOrder(Guid id);
        bool ChangeStatus(Guid id, string status);
        bool ExternalCustomerExists(Guid externalCustomerId);
    }
}