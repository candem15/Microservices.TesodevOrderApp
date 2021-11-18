using System;
using System.Collections.Generic;
using AutoMapper;
using CustomerService.Grpc;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using OrderService.Models;

namespace OrderService.SyncDataServices.Grpc
{
    public class CustomerDataClient : ICustomerDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public CustomerDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }
        public IEnumerable<Customer> ReturnAllCustomers()
        {
            Console.WriteLine($"--> Calling Grpc Service: {_configuration["GrpcCustomer"]}");
            var channel = GrpcChannel.ForAddress(_configuration["GrpcCustomer"]);
            var client = new GrpcCustomer.GrpcCustomerClient(channel);
            var request = new GetAllRequests();

            try
            {
                var reply = client.GetAllCustomers(request);
                return _mapper.Map<IEnumerable<Customer>>(reply.Customer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call Grpc Server: {ex.Message}");
                return null;
            }
        }
    }
}