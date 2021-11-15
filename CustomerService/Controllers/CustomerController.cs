using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CustomerService.AsyncDataServices;
using CustomerService.Data;
using CustomerService.Dtos;
using CustomerService.Models;
using CustomerService.SyncDataServices.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderDataClient _orderDataClient;
        private readonly IMessageBusClient _messageBusClient;
        private readonly ICustomerRepo _repository;
        public CustomerController(ICustomerRepo repository, IMapper mapper, IOrderDataClient orderDataClient, IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _orderDataClient = orderDataClient;
            _messageBusClient=messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CustomerReadDto>> GetAllCustomers()
        {
            Console.WriteLine("--> Getting all customers...");

            var customers = _repository.GetAllCustomers();

            return Ok(_mapper.Map<IEnumerable<CustomerReadDto>>(customers));
        }

        [HttpGet("{id}", Name = "GetCustomerById")]
        public ActionResult<CustomerReadDto> GetCustomerById(Guid id)
        {
            Console.WriteLine($"--> Getting customer by id : {id}");

            var customer = _repository.GetCustomerById(id);

            if (customer == null)
                return NotFound();
            return Ok(_mapper.Map<CustomerReadDto>(customer));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerReadDto>> CreateCustomer(CustomerCreateDto customerCreateDto)
        {
            var customerModel = _mapper.Map<Customer>(customerCreateDto);
            customerModel.CreatedAt=DateTime.Now;
            customerModel.UpdatedAt=DateTime.Now;
            var customerId = _repository.CreateCustomer(customerModel);
            if (customerId != null)
                Console.WriteLine($"--> New customer created with Id: {customerId}");
            var customerReadDto = _mapper.Map<CustomerReadDto>(customerModel);

            // Sync message sending
            try
            {
                await _orderDataClient.SendCustomerToOrder(customerReadDto);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"--> Could not send synchronously: {exception.Message}");
            }

            // ASync message sending
            try
            {
                var customerPublishedDto = _mapper.Map<CustomerPublishedDto>(customerReadDto);
                customerPublishedDto.Event = "Customer_Published";
                _messageBusClient.PublishNewCustomer(customerPublishedDto);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"--> Could not send asynchronously: {exception.Message}");
            }

            return CreatedAtRoute(nameof(GetCustomerById), new { Id = customerReadDto.Id }, customerReadDto); //This returns "201" with body payload of platformReadDto and Id as response.
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCustomer(Guid id)
        {
            if (_repository.DeleteCustomer(id))
                return NoContent();
            return NotFound();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCustomer(Guid id, CustomerUpdateDto customerUpdateDto)
        {
            var existingCustomer = _repository.GetCustomerById(id);

            if (existingCustomer == null)
            {
                return NotFound();
            }
            existingCustomer.Name = customerUpdateDto.Name;
            existingCustomer.Email = customerUpdateDto.Email;
            existingCustomer.UpdatedAt = DateTime.Now;
            if (customerUpdateDto.Addresses != null)
                _repository.UpdateCustomer(existingCustomer, customerUpdateDto.Addresses);
            _repository.UpdateCustomer(existingCustomer, null);
            return NoContent();
        }
        /*[HttpGet("address/{id}", Name = "GetAdressesByCustomerId")] ""Test Purposes""
        public ActionResult<Address> GetAdressesByCustomerId(Guid id)
        {
            Console.WriteLine($"--> Getting customer by id : {id}");

            var customer = _repository.GetAdressesByCustomerId(id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }*/
    }
}