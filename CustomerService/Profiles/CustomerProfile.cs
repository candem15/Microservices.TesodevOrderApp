using System.Collections.Generic;
using AutoMapper;
using CustomerService.Dtos;
using CustomerService.Grpc;
using CustomerService.Models;
using Google.Protobuf.Collections;

namespace CustomerService.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            // Source to Target
            CreateMap<Customer, CustomerReadDto>();
            CreateMap<CustomerCreateDto, Customer>();
            CreateMap<CustomerUpdateDto, Customer>();
            CreateMap<CustomerReadDto, CustomerPublishedDto>();
            CreateMap<Customer, GrpcCustomerModel>()
                 .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.Id))
                 .ForMember(d => d.Addresses, opt => opt.MapFrom(d => d.Addresses));
            CreateMap<Models.Address,Grpc.Address>();
        }
    }
}