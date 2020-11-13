using Microsoft.AspNetCore.Mvc;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Cryptocop.Software.API.Controllers
{
    [Authorize]
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetOrders()
        // Gets all orders associated with an authenticated user
        {
            return Ok(_orderService.GetOrders(User.Identity.Name));
        }

        [HttpPost]
        [Route("")]
        public IActionResult CreateNewOrder([FromBody] OrderInputModel order)
        // Adds a new order associated with an authenticated user
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(412, order);
            }

            _orderService.CreateNewOrder(User.Identity.Name, order);
            return StatusCode(201, "Order has been created.");
        }
    }
}