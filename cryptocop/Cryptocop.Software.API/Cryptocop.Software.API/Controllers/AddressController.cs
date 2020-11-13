using Microsoft.AspNetCore.Mvc;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Cryptocop.Software.API.Controllers
{
    [Authorize]
    [Route("api/addresses")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetAllAddresses()
        // Gets all addresses associated with an authenticated user
        {
            return Ok(_addressService.GetAllAddresses(User.Identity.Name));
        }

        [HttpPost]
        [Route("")]
        public IActionResult AddAddress([FromBody] AddressInputModel address)
        // Adds a new address associated with an authenticated user
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(412, address);
            }
            _addressService.AddAddress(User.Identity.Name, address);
            return StatusCode(201, "Address has been created.");
        }

        [HttpDelete]
        [Route("{addressId}")]
        public IActionResult DeleteAddress(int addressId)
        // Adds a new address associated with an authenticated user
        {
            _addressService.DeleteAddress(User.Identity.Name, addressId);
            return NoContent();
        }
    }
}