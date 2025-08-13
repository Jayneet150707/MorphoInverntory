using Microsoft.AspNetCore.Mvc;
using MorphoInventoryApi.Models;
using MorphoInventoryApi.Services;

namespace MorphoInventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DeviceOrderRequest>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<DeviceOrderRequest>> GetOrderById(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            
            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found");
            }
            
            return Ok(order);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<DeviceOrderRequest>>> GetOrdersByStatus(string status)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var orderId = await _orderService.CreateOrderAsync(request);
            return CreatedAtAction(nameof(GetOrderById), new { orderId }, orderId);
        }

        [HttpPost("approve")]
        public async Task<ActionResult> ApproveOrder([FromBody] ApproveOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var order = await _orderService.GetOrderByIdAsync(request.OrderId);
            if (order == null)
            {
                return NotFound($"Order with ID {request.OrderId} not found");
            }
            
            if (order.Status != "Pending")
            {
                return BadRequest($"Order with ID {request.OrderId} is not in Pending status");
            }
            
            var success = await _orderService.ApproveOrderAsync(request);
            
            if (!success)
            {
                return StatusCode(500, "Failed to update order status");
            }
            
            return NoContent();
        }

        [HttpPut("{orderId}/status/{status}")]
        public async Task<ActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found");
            }
            
            var success = await _orderService.UpdateOrderStatusAsync(orderId, status);
            
            if (!success)
            {
                return StatusCode(500, "Failed to update order status");
            }
            
            return NoContent();
        }
    }
}

