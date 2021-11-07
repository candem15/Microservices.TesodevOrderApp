using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CustomerService.Dtos;
using Microsoft.Extensions.Configuration;

namespace CustomerService.SyncDataServices.Http
{
    public class HttpOrderDataClient : IOrderDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpOrderDataClient(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient=httpClient;
            _configuration=configuration;
        }
        public async Task SendCustomerToOrder(CustomerReadDto customerReadDto)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(customerReadDto),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_configuration["OrderService"]}",httpContent);

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Synchronous POST to OrderService is success!");
            }
            else
            {
                 Console.WriteLine("--> Synchronous POST to OrderService is failed!");
            }
        }
    }
}