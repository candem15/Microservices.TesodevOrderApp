using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CustomerService.Dtos;
using CustomerService.Models;

namespace CustomerService.Profiles
{
    public class CustomerProfile : Profile
    {
         public CustomerProfile()
         {
            // Source to Target
            CreateMap<Customer,CustomerReadDto>();
            CreateMap<CustomerCreateDto,Customer>();
            CreateMap<CustomerUpdateDto,Customer>();
         }
    }
}