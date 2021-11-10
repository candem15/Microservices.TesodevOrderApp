using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrderService.Data;
using OrderService.Dtos;
using OrderService.Models;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepo _repository;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<OrderReadDto>> GetAllOrders()
        {
            Console.WriteLine("--> Getting all orders...");

            var orders = _repository.GetAllOrders();

            return Ok(_mapper.Map<IEnumerable<OrderReadDto>>(orders));
        }

        [HttpGet("customer/{id}")]
        public ActionResult<IEnumerable<OrderReadDto>> GetAllOrdersByCustomerId(Guid id)
        {
            Console.WriteLine($"--> Getting orders by CustomerId : {id}");

            var orders = _repository.GetAllOrdersByCustomerId(id);

            if (orders == null)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<OrderReadDto>>(orders));
        }

        [HttpGet("{id}", Name = "GetOrderById")]
        public ActionResult<OrderReadDto> GetOrderById(Guid id)
        {
            Console.WriteLine($"--> Getting order by Id : {id}");

            var order = _repository.GetOrderById(id);

            if (order == null)
                return NotFound();

            return Ok(_mapper.Map<OrderReadDto>(order));
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteOrder(Guid id)
        {
            if (_repository.DeleteOrder(id))
                return NoContent();
            return NotFound();
        }

        [HttpPost]
        public ActionResult<OrderReadDto> CreateOrder(OrderCreateDto orderCreateDto)
        {
            var orderModel = _mapper.Map<Order>(orderCreateDto);
            var orderId = _repository.CreateOrder(orderModel);
            if (orderId != null)
                Console.WriteLine($"--> New order created with Id: {orderId}");
            var orderReadDto = _mapper.Map<OrderReadDto>(orderModel);

            return CreatedAtRoute(nameof(GetOrderById), new { Id = orderReadDto.Id }, orderReadDto);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateOrder(Guid id, OrderUpdateDto orderUpdateDto)
        {
            var existingOrder = _repository.GetOrderById(id);
            if (existingOrder == null)
            {
                Console.WriteLine($"--> Order with id:{id} not exists!");
                return NotFound();
            }
            existingOrder.Price = orderUpdateDto.Price;
            existingOrder.Quantity = orderUpdateDto.Quantity;
            existingOrder.Products = orderUpdateDto.Products;
            existingOrder.UpdatedAt = DateTime.Now;

            if (orderUpdateDto.Products != null)
                _repository.UpdateOrder(existingOrder, orderUpdateDto.Products);
            _repository.UpdateOrder(existingOrder, null);

            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult<OrderReadDto> ChangeStatus(Guid id, OrderUpdateDto orderUpdateDto)
        {
            var status = JsonConvert.DeserializeObject<string>(orderUpdateDto.Status);

            if(_repository.ChangeStatus(id,status))
                return GetOrderById(id);

            return BadRequest();
        }

    }
}