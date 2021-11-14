using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderService.Controllers;
using OrderService.Data;
using OrderService.Dtos;
using OrderService.Models;
using OrderService.Profiles;
using Xunit;

namespace UnitTests.OrderServiceTests
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderRepo> repositoryStub = new();
        private readonly Random rand = new();
        private IMapper mapperObject()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new OrderProfile());
            });
            var mapper = mockMapper.CreateMapper();

            return mapper;
        }

        [Fact]
        public void GetOderById_WithUnexistingOrder_ReturnsNotFound()
        {
            //Arrange

            repositoryStub.Setup(repo => repo.GetOrderById(It.IsAny<Guid>()))
                .Returns((Order)null);

            var controller = new OrderController(repositoryStub.Object, mapperObject());

            //Act

            var result = controller.GetOrderById(Guid.NewGuid());

            //Assert

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void GetOrderById_WithExistingOrder_ReturnsExpectedCustomer()
        {
            //Arrange

            Order expectedOrder = CreateRandomOrder();

            repositoryStub.Setup(repo => repo.GetOrderById(It.IsAny<Guid>()))
                .Returns(expectedOrder);

            var controller = new OrderController(repositoryStub.Object, mapperObject());

            //Act

            var result = controller.GetOrderById(Guid.NewGuid());

            //Assert

            var resultObject = (result.Result as OkObjectResult).Value as OrderReadDto;
            resultObject.Should().BeEquivalentTo(expectedOrder, opt => opt.ComparingByMembers<Order>().ExcludingMissingMembers());

        }

        [Fact]
        public void GetAllOrders_WithExistingOrders_ReturnsAllOrders()
        {
            //Arrange

            IEnumerable<Order> expectedCustomers = new[] { CreateRandomOrder(), CreateRandomOrder(), CreateRandomOrder() };

            repositoryStub.Setup(repo => repo.GetAllOrders())
                .Returns(expectedCustomers);

            var controller = new OrderController(repositoryStub.Object, mapperObject());

            //Act

            var result = controller.GetAllOrders();

            //Assert

            var resultObjects = (result.Result as OkObjectResult).Value as IEnumerable<OrderReadDto>;
            resultObjects.Should().BeEquivalentTo(expectedCustomers, opt => opt.ComparingByMembers<Order>().ExcludingMissingMembers());

        }

        [Fact]
        public void CreateOrder_WithOrderToCreate_ReturnsCreatedOrder()
        {
            //Arrange

            var orderToCreate = CreateRandomOrder();
            var orderCreateModel = new OrderCreateDto
            {
                CustomerId = orderToCreate.CustomerId,
                Address = new Address()
                {
                    Id = orderToCreate.Address.Id,
                    AddressLine = orderToCreate.Address.AddressLine,
                    City = orderToCreate.Address.City,
                    Country = orderToCreate.Address.Country,
                    CityCode = 42
                },
                Quantity = orderToCreate.Quantity,
                Price = orderToCreate.Price,
                Status = orderToCreate.Status,
                Product = new Product()
                {
                    Id = orderToCreate.Product.Id,
                    Name = orderToCreate.Product.Name,
                    ImageUrl = orderToCreate.Product.ImageUrl
                }
            };
            repositoryStub.Setup(repo => repo.CreateOrder(orderToCreate)).Returns(Guid.NewGuid());
            var controller = new OrderController(repositoryStub.Object, mapperObject());

            //Act

            var result = controller.CreateOrder(orderCreateModel);

            //Assert

            var createdOrder = (result.Result as CreatedAtRouteResult).Value as OrderReadDto;
            orderToCreate.Should().BeEquivalentTo(
                createdOrder,
                options => options.ComparingByMembers<Order>().ExcludingMissingMembers().Excluding(p=>p.Id).Excluding(p=>p.CreatedAt).Excluding(p=>p.UpdatedAt)
                );
            createdOrder.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(1000));
            createdOrder.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(1000));
        }
        /* InProgresss
                                [Fact]
                                public void UpdateCustomer_WithExistingCustomer_ReturnsNoContent()
                                {
                                    //Arrange

                                    Customer existingCustomer = CreateRandomCustomer();
                                    repositoryStub.Setup(repo => repo.GetCustomerById(It.IsAny<Guid>()).Equals(true));

                                    var customerId = existingCustomer.Id; // We took existingCustomer's id for update.
                                    var customerToUpdate = new CustomerUpdateDto()
                                    { //Here we give new values to properties for simulate updating customer.
                                        Name = Guid.NewGuid().ToString(),
                                        Email = Guid.NewGuid().ToString(),
                                        Addresses = new Address()
                                        {
                                            Id = Guid.NewGuid(),
                                            AddressLine = Guid.NewGuid().ToString(),
                                            City = Guid.NewGuid().ToString(),
                                            Country = Guid.NewGuid().ToString(),
                                            CityCode = 42
                                        }
                                    };

                                    var controller = new CustomerController(repositoryStub.Object, mapperObject(), orderDataClientStub.Object);

                                    //Act

                                    var result = controller.UpdateCustomer(customerId, customerToUpdate);

                                    //Assert

                                    result.Should().BeOfType<NoContentResult>();

                                }

                                [Fact]
                                public void DeleteCustomer_WithExistingCustomer_ReturnsNoContent()
                                {
                                    //Arrange

                                    Customer existingCustomer = CreateRandomCustomer();
                                    repositoryStub.Setup(repo => repo.GetCustomerById(It.IsAny<Guid>()))
                                        .Returns(existingCustomer);
                                    repositoryStub.Setup(repo => repo.ValidateCustomer(It.IsAny<Guid>()))
                                        .Returns(true);
                                    repositoryStub.Setup(repo => repo.DeleteCustomer(It.IsAny<Guid>()))
                                        .Returns(true);
                                    var customerId = existingCustomer.Id;

                                    var controller = new CustomerController(repositoryStub.Object, mapperObject(), orderDataClientStub.Object);

                                    //Act

                                    var result = controller.DeleteCustomer(customerId);

                                    //Assert

                                    result.Should().BeOfType<NoContentResult>();

                                }
                        */
        private Order CreateRandomOrder()
        {
            return new() //We don't care about much what are the values of properties. But we have to give values right type and range like actual "Customer".
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Address = new Address()
                {
                    Id = Guid.NewGuid(),
                    AddressLine = Guid.NewGuid().ToString(),
                    City = Guid.NewGuid().ToString(),
                    Country = Guid.NewGuid().ToString(),
                    CityCode = 42
                },
                Quantity = rand.Next(1000),
                Price = Convert.ToDouble(rand.Next(1000)),
                Status = Guid.NewGuid().ToString(),
                Product = new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = Guid.NewGuid().ToString(),
                    ImageUrl = Guid.NewGuid().ToString()
                },
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }
    }
}