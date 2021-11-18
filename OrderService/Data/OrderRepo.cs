using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.Data
{
    public class OrderRepo<T> : IOrderRepo<T> where T : class
    {
        private readonly AppDbContext _context;

        public OrderRepo(AppDbContext context)
        {
            _context = context;
        }
        public bool ChangeStatus(Guid id, string status)
        {
            var existingOrder = _context.Orders.FirstOrDefault(p => p.Id == id);
            if (existingOrder == null)
            {
                Console.WriteLine($"--> Status of order could not changed with given Id:{id}");
                return false;
            }

            existingOrder.Status = status;
            existingOrder.UpdatedAt = DateTime.Now;

            _context.Orders.Update(existingOrder);
            SaveChanges();
            Console.WriteLine($"--> Status of order successfully changed with given Id:{id}");
            return true;
        }

        public void CreateCustomer(Customer customer)
        {
            if (customer == null)
            {
                string nullException = new ArgumentNullException(nameof(customer)).ToString();
                Console.WriteLine($"--> Order could not created : {nullException}");
            }
            _context.Customers.Add(customer);
            SaveChanges();

            Console.WriteLine($"--> Customer created successfully with given Id:{customer.Id}");
        }

        public Guid CreateOrder(Order order)
        {
            if (order == null)
            {
                string nullException = new ArgumentNullException(nameof(order)).ToString();
                Console.WriteLine($"--> Order could not created : {nullException}");
                return Guid.Empty;
            }
            _context.Orders.Add(order);
            SaveChanges();
            Console.WriteLine($"--> Order created successfully with given Id:{order.Id}");
            return order.Id;
        }

        public bool DeleteOrder(Guid id)
        {
            var existingOrder = GetOrderById(id);
            if (existingOrder == null)
            {
                Console.WriteLine($"--> Order could not found with Id:{id}");
                return false;
            }
            _context.Orders.Remove(existingOrder);
            SaveChanges();

            Console.WriteLine($"--> Order deleted successfuly with Id:{id}");
            return true;
        }

        public bool ExternalCustomerExists(Guid externalCustomerId)
        {
            return _context.Customers.Any(p => p.ExternalID == externalCustomerId);
        }

        public IEnumerable<Order> GetAllOrders()
        {
            var result = (from ord in _context.Orders.ToList()
                          join adr in _context.Addresses.ToList() on ord.AddressId equals adr.Id
                          join pro in _context.Products.ToList() on ord.ProductId equals pro.Id
                          select ord).ToList();

            return result;
        }

        public IEnumerable<Order> GetAllOrdersByCustomerId(Guid id)
        {
            var result = (from ord in _context.Orders.ToList()
                          join adr in _context.Addresses.ToList() on ord.AddressId equals adr.Id
                          join pro in _context.Products.ToList() on ord.ProductId equals pro.Id
                          select ord).Where(p => p.CustomerId == id).ToList();

            return result;
        }

        public Order GetOrderById(Guid id)
        {
            return _context.Orders.FirstOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool UpdateOrder(Order order)
        {
            if (_context.Orders.FirstOrDefault(p => p.Id == order.Id) == null)
            {
                Console.WriteLine("--> Order could not updated");
                return false;
            }
            _context.Orders.Update(order);
            if (order.Product != null)
            {
                order.Product.Id = order.ProductId;
                _context.Products.Update(order.Product);
            }
            if (order.Address != null)
            {
                order.Address.Id = order.AddressId;
                _context.Addresses.Update(order.Address);
            }
            SaveChanges();
            Console.WriteLine($"--> Order updated successfully with Id:{order.Id}");
            return true;
        }
    }
}