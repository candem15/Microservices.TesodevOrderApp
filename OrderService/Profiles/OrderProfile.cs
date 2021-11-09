using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OrderService.Dtos;
using OrderService.Models;

namespace OrderService.Profiles
{
    public class OrderProfile:Profile
    {
         public OrderProfile()
         {
            // Source to Target
            CreateMap<Order,OrderReadDto>();
            CreateMap<OrderCreateDto,Order>();
            CreateMap<OrderUpdateDto,Order>();
            CreateMap<CustomerPublishedDto, Customer>()
                .ForMember(
                    destinationMember => destinationMember.ExternalID,
                    memberOption => memberOption.MapFrom(
                        sourceMember => sourceMember.Id));
         }

    }
}