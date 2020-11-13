using System.Threading.Tasks;
using Cryptocop.Software.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cryptocop.Software.API.Controllers
{

    [Authorize]
    [Route("api/exchanges")]
    [ApiController]
    public class ExchangeController : ControllerBase
    {
        private readonly IExchangeService _exchangeService;

        public ExchangeController(IExchangeService exchangeService)
        {
            _exchangeService = exchangeService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAllExchanges([FromQuery] int pageNumber = 1)
        // Gets all exchanges in a paginated envelope
        // Accepts a single query parameter called pageNumber
        {
            return Ok(await _exchangeService.GetExchanges(pageNumber));
        }
    }
}