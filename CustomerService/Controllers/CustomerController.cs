using System;
using System.Collections.Generic;
using AutoMapper;
using CustomerService.Data;
using CustomerService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepo _repository;
        public CustomerController(ICustomerRepo repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CustomerReadDto>> GetAllCustomers()
        {
            Console.WriteLine("--> Getting all customers...");

            var customerItem=_repository.GetAllCustomers();

            return Ok(_mapper.Map<IEnumerable<CustomerReadDto>>(customerItem));
        }
    }
}