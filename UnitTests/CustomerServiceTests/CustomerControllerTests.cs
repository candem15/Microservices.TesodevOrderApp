using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using CustomerService.Controllers;
using CustomerService.Data;
using CustomerService.Models;
using AutoMapper;
using CustomerService.Profiles;
using CustomerService.SyncDataServices.Http;
using CustomerService.Dtos;

namespace UnitTests.CustomerServiceTests
{
    public class ItemsControllerTests
    {
        private readonly Mock<ICustomerRepo> repositoryStub = new();
        private readonly Mock<IOrderDataClient> orderDataClientStub = new();
        private IMapper mapperObject()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CustomerProfile());
            });
            var mapper = mockMapper.CreateMapper();

            return mapper;
        }

        [Fact]
        public void GetCustomerById_WithUnexistingCustomer_ReturnsNotFound()
        {
            //Arrange =>  In this section we set up everything to be ready to execute the test.

            repositoryStub.Setup(repo => repo.GetCustomerById(It.IsAny<Guid>()))
                .Returns((Customer)null);

            var controller = new CustomerController(repositoryStub.Object, mapperObject(), orderDataClientStub.Object, null);

            //Act => In this section we execute the test.

            var result = controller.GetCustomerById(Guid.NewGuid());

            //Assert => In this section we verify whatever needs to be verified about the execution of the unit test.

            result.Result.Should().BeOfType<NotFoundResult>(); //This is fluent version of assert.
        }

        [Fact]
        public void GetCustomerById_WithExistingCustomer_ReturnsExpectedCustomer()
        {
            //Arrange

            Customer expectedCustomer = CreateRandomCustomer();

            repositoryStub.Setup(repo => repo.GetCustomerById(It.IsAny<Guid>()))
                .Returns(expectedCustomer);

            var controller = new CustomerController(repositoryStub.Object, mapperObject(), orderDataClientStub.Object, null);

            //Act

            var result = controller.GetCustomerById(Guid.NewGuid());

            //Assert

            var resultObject = (result.Result as OkObjectResult).Value as CustomerReadDto;
            resultObject.Should().BeEquivalentTo(expectedCustomer);

        }

        [Fact]
        public void GetAllCustomers_WithExistingCustomers_ReturnsAllCustomers()
        {
            //Arrange

            IEnumerable<Customer> expectedCustomers = new[] { CreateRandomCustomer(), CreateRandomCustomer(), CreateRandomCustomer() };

            repositoryStub.Setup(repo => repo.GetAllCustomers())
                .Returns(expectedCustomers);

            var controller = new CustomerController(repositoryStub.Object, mapperObject(), orderDataClientStub.Object, null);

            //Act

            var result = controller.GetAllCustomers();

            //Assert

            var resultObjects = (result.Result as OkObjectResult).Value as IEnumerable<CustomerReadDto>;
            resultObjects.Should().BeEquivalentTo(expectedCustomers);

        }

        [Fact]
        public void CreateCustomer_WithCustomerToCreate_ReturnsCreatedCustomer()
        {
            //Arrange

            var customerToCreate = CreateRandomCustomer();
            var customerCreateModel = new CustomerCreateDto
            {
                Name = customerToCreate.Name,
                Email = customerToCreate.Email,
                Addresses = customerToCreate.Addresses
            };
            repositoryStub.Setup(repo => repo.CreateCustomer(customerToCreate)).Returns(Guid.NewGuid());
            var controller = new CustomerController(repositoryStub.Object, mapperObject(), orderDataClientStub.Object, null);

            //Act

            var result = controller.CreateCustomer(customerCreateModel).Result;

            //Assert

            var createdCustomer = (result.Result as CreatedAtRouteResult).Value as CustomerReadDto;
            customerToCreate.Should().BeEquivalentTo(
                createdCustomer,
                options => options.ComparingByMembers<Customer>()
                    .Excluding(p => p.Id)
                    .Excluding(p => p.CreatedAt)
                    .Excluding(p => p.UpdatedAt)
                );
            createdCustomer.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(1000));
            createdCustomer.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(1000));
        }

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

            var controller = new CustomerController(repositoryStub.Object, mapperObject(), orderDataClientStub.Object, null);

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

            var controller = new CustomerController(repositoryStub.Object, mapperObject(), orderDataClientStub.Object, null);

            //Act

            var result = controller.DeleteCustomer(customerId);

            //Assert

            result.Should().BeOfType<NoContentResult>();

        }

        private Customer CreateRandomCustomer()
        {
            return new() //We don't care about much what are the values of properties. But we have to give values right type and range like actual "Customer".
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Addresses = {
                    new Address(){
                        Id=Guid.NewGuid(),
                        AddressLine=Guid.NewGuid().ToString(),
                        City=Guid.NewGuid().ToString(),
                        Country=Guid.NewGuid().ToString(),
                        CityCode=42 },
                    new Address(){
                        Id=Guid.NewGuid(),
                        AddressLine=Guid.NewGuid().ToString(),
                        City=Guid.NewGuid().ToString(),
                        Country=Guid.NewGuid().ToString(),
                        CityCode=42 }
                },
                Email = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }
    }
}
