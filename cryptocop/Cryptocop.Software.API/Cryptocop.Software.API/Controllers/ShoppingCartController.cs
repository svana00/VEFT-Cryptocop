using Microsoft.AspNetCore.Mvc;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Cryptocop.Software.API.Controllers
{
    [Authorize]
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetCartItems(int cartItemId)
        // Deletes an item from a shopping cart by id
        {
            var cartItems = _shoppingCartService.GetCartItems(User.Identity.Name);
            return Ok(cartItems);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> AddItemToCart([FromBody] ShoppingCartItemInputModel shoppingCartItem)
        // Adds an item to the shopping cart
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(412, shoppingCartItem);
            }
            await _shoppingCartService.AddCartItem(User.Identity.Name, shoppingCartItem);
            return StatusCode(201, "Item has been added to the shopping cart.");
        }

        [HttpDelete]
        [Route("{cartItemId}")]
        public IActionResult DeleteItemFromCart(int cartItemId)
        // Deletes an item from a shopping cart by id
        {
            _shoppingCartService.RemoveCartItem(User.Identity.Name, cartItemId);
            return NoContent();
        }

        [HttpPatch]
        [Route("{cartItemId}")]
        public IActionResult UpdateQuantityOfItem([FromBody] ShoppingCartItemInputModel shoppingCartItem, int cartItemId)
        // Updates the quantity for an item in the cart
        {
            _shoppingCartService.UpdateCartItemQuantity(User.Identity.Name, cartItemId, (float)shoppingCartItem.Quantity);
            return NoContent();
        }

        [HttpDelete]
        [Route("")]
        public IActionResult ClearCart()
        // Clears the cart
        // All items within the cart should be deleted
        {
            _shoppingCartService.ClearCart(User.Identity.Name);
            return NoContent();
        }
    }
}