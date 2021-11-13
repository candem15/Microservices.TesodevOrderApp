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
        private readonly Mock<ILogger<CustomerController>> loggerStub = new();
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

            var controller = new CustomerController(repositoryStub.Object, null, null);

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

            var controller = new CustomerController(repositoryStub.Object, mapperObject(), orderDataClientStub.Object);

            //Act

            var result = controller.GetCustomerById(Guid.NewGuid()).Value;

            //Assert

            result.Should().BeEquivalentTo(expectedCustomer);

        }

        [Fact]
        public void GetAllCustomers_WithExistingCustomers_ReturnsAllCustomers()
        {
            //Arrange

            IEnumerable<Customer> expectedCustomers = new[] { CreateRandomCustomer(), CreateRandomCustomer(), CreateRandomCustomer() };

            repositoryStub.Setup(repo => repo.GetAllCustomers())
                .Returns(expectedCustomers);

            var controller = new CustomerController(repositoryStub.Object, mapperObject(), orderDataClientStub.Object);

            //Act

            var result = controller.GetAllCustomers().Value;

            //Assert

            result.Should().BeEquivalentTo(expectedCustomers);

        }

        [Fact] // STILL IN PROGRESSSSSS!!!
        public void CreateCustomer_WithCustomerToCreate_ReturnsCreatedCustomer()
        {
            //Arrange

            var customerToCreate = CreateRandomCustomer();
            var customerCreateModel = new CustomerCreateDto {
                Name = customerToCreate.Name,
                Email = customerToCreate.Email,
                Addresses = customerToCreate.Addresses
                };
            repositoryStub.Setup(repo => repo.CreateCustomer(customerToCreate)).Returns(customerToCreate.Id);
            var controller = new CustomerController(repositoryStub.Object, mapperObject(), orderDataClientStub.Object);

            //Act

            var result = controller.CreateCustomer(customerCreateModel).Result.Value;

            //Assert

            var createdCustomer = (result.Result as CreatedAtRouteResult).Value as CustomerReadDto;
            customerToCreate.Should().BeEquivalentTo(
                createdCustomer,
                options => options.ComparingByMembers<Customer>().ExcludingMissingMembers() //"ItemDto" at Catalog.Api has 4 properties but "itemToCreate" got only 2. Because of that added additional filter for comparing without missing members.
                );
            createdCustomer.Id.Should().NotBeEmpty();
            createdCustomer.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(1000));
            createdCustomer.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(1000));
        }
        /*
                                [Fact]
                                public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
                                {
                                    //Arrange

                                    Item existingItem = CreateRandomItem();
                                    repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                                        .ReturnsAsync(existingItem);

                                    var itemId = existingItem.Id; // We took existingItem's id for update.
                                    var itemToUpdate = new UpdateItemDto( //Here we give new values to properties for simulate updating item.
                                        Guid.NewGuid().ToString(),
                                        rand.Next(1000),
                                        Guid.NewGuid().ToString());

                                    var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

                                    //Act

                                    var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

                                    //Assert

                                    result.Should().BeOfType<NoContentResult>();

                                }

                                [Fact]
                                public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
                                {
                                    //Arrange

                                    Item existingItem = CreateRandomItem();
                                    repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                                        .ReturnsAsync(existingItem);

                                    var itemId = existingItem.Id; // We took existingItem's id for delete.

                                    var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

                                    //Act

                                    var result = await controller.DeleteItemAsync(itemId);

                                    //Assert

                                    result.Should().BeOfType<NoContentResult>();

                                }
                        */
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
//There is extension to visualize the overall status of our test suite. Like, which case is passing, which words are failing and so on. To do that we added ".NET Core Test Explorer" extension. Then click gear icon and go extension settings, switch to workspace section from user and find "dotnet-test-explorer:Test project path" and write inside of it => "**/*Tests.csproj".
//Added "dotnet add package FluentAssertions" library to Catalog.UnitTests project. In our case this library will helps us compare existingItem and itemDto without code repeat.