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
            CreateMap<CustomerReadDto,CustomerPublishedDto>();
            CreateMap<Customer,GrpcCustomerModel>()
                .ForMember(d=>d.CustomerId,opt=>opt.MapFrom(s=>s.Id))
                .ForMember(d=>d.Name,opt=>opt.MapFrom(s=>s.Name))
                .ForMember(d=>d.Email,opt=>opt.MapFrom(s=>s.Email))
                .ForMember(d=>d.Addresses,opt=>opt.MapFrom(s=>s.Addresses));
         }
    }
}