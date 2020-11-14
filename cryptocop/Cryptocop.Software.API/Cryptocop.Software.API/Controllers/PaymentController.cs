using Microsoft.AspNetCore.Mvc;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Cryptocop.Software.API.Controllers
{
    [Authorize]
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetStoredPaymentCards()
        // Gets all payment cards associated with an authenticated user
        {
            return Ok(_paymentService.GetStoredPaymentCards(User.Identity.Name));
        }

        [HttpPost]
        [Route("")]
        public IActionResult AddPaymentCard([FromBody] PaymentCardInputModel paymentCard)
        // Adds a new payment card associated with an authenticated user
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(412, paymentCard);
            }

            _paymentService.AddPaymentCard(User.Identity.Name, paymentCard);
            return StatusCode(201, "Payment card has been added.");
        }
    }
}