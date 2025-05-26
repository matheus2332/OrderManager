using Microsoft.AspNetCore.Mvc;
using OrderManager.API.Models;
using OrderManager.Application.Interfaces;

namespace OrderManager.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController(IOrderService orderService) : ControllerBase
    {
        [HttpPost]
        public IActionResult Create([FromBody] CreateOrderRequest request)
        {
            var order = orderService.CreateOrder(request.Quantity);
            return Ok(order);
        }

        [HttpPost("{id}/complete")]
        public IActionResult Complete(int id)
        {
            if (orderService.CompleteOrder(id)) return Ok($"order {id} completed");
            return NotFound("Order not found");
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(orderService.GetAll());
    }
}
