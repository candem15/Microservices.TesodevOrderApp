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
        public void GetAllOrdersByCustomerId_WithExistingOrders_ReturnsMatchingOrders()
        {
            //Arrange

            var order1 = CreateOrderByCustomerId();
            var order2 = CreateOrderByCustomerId();
            IEnumerable<Order> ordersDatabase = new[] { CreateRandomOrder(), order1, order2 };
            IEnumerable<Order> expectedOrders = new[] { order1, order2 };
            Guid customerIdToGetOrders = new Guid("8d207077-9e8b-4dbb-a30e-7bb2a2ac7893");
            repositoryStub.Setup(repo => repo.GetAllOrdersByCustomerId(customerIdToGetOrders))
                .Returns(ordersDatabase.Where(p => p.CustomerId == customerIdToGetOrders));

            var controller = new OrderController(repositoryStub.Object, mapperObject());

            //Act

            var result = controller.GetAllOrdersByCustomerId(customerIdToGetOrders);

            //Assert

            var resultObjects = (result.Result as OkObjectResult).Value as IEnumerable<OrderReadDto>;
            resultObjects.Should().BeEquivalentTo(expectedOrders, opt => opt.ComparingByMembers<Order>().ExcludingMissingMembers());

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
                options => options.ComparingByMembers<Order>().ExcludingMissingMembers().Excluding(p => p.Id).Excluding(p => p.CreatedAt).Excluding(p => p.UpdatedAt)
                );
            createdOrder.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(1000));
            createdOrder.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(1000));
        }

        [Fact]
        public void UpdateOrder_WithExistingOrder_ReturnsNoContent()
        {
            //Arrange

            Order existingOrder = CreateRandomOrder();
            repositoryStub.Setup(repo => repo.GetOrderById(It.IsAny<Guid>()).Equals(true));

            var orderId = existingOrder.Id;
            var orderToUpdate = new OrderUpdateDto()
            {
                CustomerId = Guid.NewGuid(),
                Address = new Address()
                {
                    Id = Guid.NewGuid(),
                    AddressLine = Guid.NewGuid().ToString(),
                    City = Guid.NewGuid().ToString(),
                    Country = Guid.NewGuid().ToString(),
                    CityCode = rand.Next(100)
                },
                Quantity = rand.Next(100),
                Price = rand.Next(100),
                Status = Guid.NewGuid().ToString(),
                Product = new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = Guid.NewGuid().ToString(),
                    ImageUrl = Guid.NewGuid().ToString()
                }
            };

            var controller = new OrderController(repositoryStub.Object, mapperObject());

            //Act

            var result = controller.UpdateOrder(orderId, orderToUpdate);

            //Assert

            result.Should().BeOfType<NoContentResult>();

        }

        [Fact]
        public void DeleteOrder_WithExistingOrder_ReturnsNoContent()
        {
            //Arrange

            Order existingOrder = CreateRandomOrder();
            var orderId = existingOrder.Id;

            repositoryStub.Setup(repo => repo.GetOrderById(orderId))
                .Returns(existingOrder);
            repositoryStub.Setup(repo => repo.DeleteOrder(It.IsAny<Guid>()))
                .Returns(true);

            var controller = new OrderController(repositoryStub.Object, mapperObject());

            //Act

            var result = controller.DeleteOrder(orderId);

            //Assert

            result.Should().BeOfType<NoContentResult>();

        }

        private Order CreateRandomOrder()
        {
            return new()
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

        private Order CreateOrderByCustomerId()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                CustomerId = new Guid("8d207077-9e8b-4dbb-a30e-7bb2a2ac7893"),
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