using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.Dtos;

namespace CustomerService.SyncDataServices.Http
{
    public interface IOrderDataClient
    {
        Task SendCustomerToOrder(CustomerReadDto customerReadDto);
    }
}