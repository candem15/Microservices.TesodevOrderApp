using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Models;

namespace OrderService.Data
{
    public class OrderRepo : IOrderRepo
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

        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders.ToList();
        }

        public IEnumerable<Order> GetAllOrdersByCustomerId(Guid id)
        {
            return _context.Orders.Where(p => p.CustomerId == id).ToList();
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