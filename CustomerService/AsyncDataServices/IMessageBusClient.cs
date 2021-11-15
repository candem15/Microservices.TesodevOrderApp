using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.Dtos;

namespace CustomerService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void PublishNewCustomer(CustomerPublishedDto customerPublishedDto);
    }
}