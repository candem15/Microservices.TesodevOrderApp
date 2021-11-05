using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CustomerService.Data;
using CustomerService.Dtos;
using CustomerService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepo _repository;
        public CustomerController(ICustomerRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
        public ActionResult<CustomerReadDto> CreateCustomer(CustomerCreateDto customerCreateDto)
        {
            Console.WriteLine("--> New customer created!");
            var customerModel = _mapper.Map<Customer>(customerCreateDto);
            _repository.CreateCustomer(customerModel);

            var customerReadDto = _mapper.Map<CustomerReadDto>(customerModel);

            return CreatedAtRoute(nameof(GetCustomerById), new { Id = customerReadDto.Id }, customerReadDto); //This returns "201" with body payload of platformReadDto and Id as response.
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCustomer(Guid id)
        {
            var existingCustomer = _repository.GetCustomerById(id); // Find existing customer from repository.

            if(existingCustomer==null)
            {
                return NotFound();
            }

            Console.WriteLine("--> Customer deleted!");
            _repository.DeleteCustomer(id);

            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCustomer(Guid id, CustomerUpdateDto customerUpdateDto)
        {
            var existingCustomer = _repository.GetCustomerById(id);

            if(existingCustomer==null)
            {
                return NotFound();
            }
            existingCustomer.Name=customerUpdateDto.Name;
            existingCustomer.Email=customerUpdateDto.Email;
            existingCustomer.CreatedAt=customerUpdateDto.CreatedAt;
            existingCustomer.UpdatedAt=DateTime.Now;
            _repository.UpdateCustomer(existingCustomer);

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