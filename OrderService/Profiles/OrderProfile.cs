using AutoMapper;
using CustomerService.Grpc;
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
            CreateMap<GrpcCustomerModel, Customer>()
                .ForMember(
                    destinationMember=>destinationMember.ExternalID,
                    opt=>opt.MapFrom(sourceMember=>sourceMember.CustomerId))
                .ForMember(
                    destinationMember=>destinationMember.Name,
                    opt=>opt.MapFrom(sourceMember=>sourceMember.Name))
                .ForMember(
                    destinationMember=>destinationMember.Email,
                    opt=>opt.MapFrom(sourceMember=>sourceMember.Email))
                .ForMember(
                    destinationMember=>destinationMember.Addresses,
                    opt=>opt.MapFrom(sourceMember=>sourceMember.Addresses))
                .ForMember(
                    destinationMember=>destinationMember.Orders,
                    opt=>opt.Ignore());
            CreateMap<CustomerService.Grpc.Address,Models.Address>();
         }
    }
}