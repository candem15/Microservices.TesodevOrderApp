using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OrderService.Data;
using OrderService.Dtos;
using OrderService.Models;

namespace OrderService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.CustomerPublished:
                    addCustomer(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event");

            var eventType = JsonConvert.DeserializeObject<GenericEventDto>(notificationMessage);

            switch (eventType.Event)
            {
                case "Customer_Published":
                    Console.WriteLine("--> Customer Published Event dedected!");
                    return EventType.CustomerPublished;
                default:
                    Console.WriteLine("--> Could not determined event!");
                    return EventType.Undetermined;
            }
        }
        private void addCustomer(string customerPublishedMessage)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IOrderRepo<Order>>();

                var customerPublishedDto = JsonConvert.DeserializeObject<CustomerPublishedDto>(customerPublishedMessage);

                try
                {
                    var customer = _mapper.Map<Customer>(customerPublishedDto);
                    if (!repo.ExternalCustomerExists(customer.ExternalID))
                    {
                        repo.CreateCustomer(customer);
                        Console.WriteLine("--> Customer added!");
                    }
                    else
                    {
                        Console.WriteLine("--> Customer already exists...");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not add Customer to database: {ex.Message}");
                }
            }
        }
    }
    enum EventType
    {
        CustomerPublished,
        Undetermined
    }
}