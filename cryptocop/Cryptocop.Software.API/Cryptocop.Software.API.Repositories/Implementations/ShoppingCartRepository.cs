using System.Collections.Generic;
using Cryptocop.Software.API.Repositories.Interfaces;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Models.DTOs;
using Cryptocop.Software.API.Repositories.Contexts;
using Cryptocop.Software.API.Models.Entities;
using Cryptocop.Software.API.Models.Exceptions;
using System.Linq;
using AutoMapper;

namespace Cryptocop.Software.API.Repositories.Implementations
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly CryptocopDbContext _dbContext;
        private IMapper _mapper;

        public ShoppingCartRepository(CryptocopDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<ShoppingCartItemDto> GetCartItems(string email)
        {
            // Find user and cart
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            var cart = _dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == user.Id);

            // If shopping cart doesn't exist create new one
            if (cart == null)
            {
                var newCart = new ShoppingCart
                {
                    UserId = user.Id,
                };
                _dbContext.ShoppingCarts.Add(newCart);
                _dbContext.SaveChanges();
            }

            cart = _dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == user.Id);

            return _dbContext.ShoppingCartItems.Where(s => s.ShoppingCartId == cart.Id).Select(s => new ShoppingCartItemDto
            {
                Id = s.Id,
                ProductIdentifier = s.ProductIdentifier,
                Quantity = s.Quantity,
                UnitPrice = s.UnitPrice,
                TotalPrice = s.UnitPrice * s.Quantity
            });
        }

        public void AddCartItem(string email, ShoppingCartItemInputModel shoppingCartItem, float priceInUsd)
        {
            // Find user and cart
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            var cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);

            // If shopping cart doesn't exist create new one
            if (cart == null)
            {
                var newCart = new ShoppingCart
                {
                    UserId = user.Id,
                };
                _dbContext.ShoppingCarts.Add(newCart);
                _dbContext.SaveChanges();
            }

            cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);

            var newCartItem = new ShoppingCartItem
            {
                ProductIdentifier = shoppingCartItem.ProductIdentifier,
                Quantity = (float)shoppingCartItem.Quantity,
                UnitPrice = priceInUsd,
                ShoppingCartId = cart.Id
            };

            _dbContext.ShoppingCartItems.Add(newCartItem);
            _dbContext.SaveChanges();
        }

        public void RemoveCartItem(string email, int id)
        {
            // Find user and cart
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            var cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);

            // If shopping cart doesn't exist create new one
            if (cart == null)
            {
                var newCart = new ShoppingCart
                {
                    UserId = user.Id,
                };
                _dbContext.ShoppingCarts.Add(newCart);
                _dbContext.SaveChanges();
            }

            cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);

            // Find the cart item and delete it
            var cartItem = _dbContext.ShoppingCartItems.FirstOrDefault(s => s.Id == id && s.ShoppingCartId == cart.Id);

            if (cartItem == null)
            {
                throw new ResourceNotFoundException($"Cart item with id {id} was not found.");
            }
            _dbContext.ShoppingCartItems.Remove(cartItem);
            _dbContext.SaveChanges();
        }

        public void UpdateCartItemQuantity(string email, int id, float quantity)
        {
            // Find user and cart
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            var cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);

            // If shopping cart doesn't exist create new one
            if (cart == null)
            {
                var newCart = new ShoppingCart
                {
                    UserId = user.Id,
                };
                _dbContext.ShoppingCarts.Add(newCart);
                _dbContext.SaveChanges();
            }

            cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);

            // Find the cart item
            var cartItem = _dbContext.ShoppingCartItems.FirstOrDefault(s => s.Id == id && s.ShoppingCartId == cart.Id);

            if (cartItem == null)
            {
                throw new ResourceNotFoundException($"Cart item with id {id} was not found.");
            }

            // Update quantity
            cartItem.Quantity = quantity;

            _dbContext.SaveChanges();
        }

        public void ClearCart(string email)
        {
            // Find user and cart
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            var cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);

            // If shopping cart doesn't exist create new one
            if (cart == null)
            {
                var newCart = new ShoppingCart
                {
                    UserId = user.Id,
                };
                _dbContext.ShoppingCarts.Add(newCart);
                _dbContext.SaveChanges();
            }

            cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);

            // Find all items in the cart and delete them
            var itemsToDelete = _dbContext.ShoppingCartItems.Where(s => s.ShoppingCartId == cart.Id);
            foreach (var item in itemsToDelete)
            {
                _dbContext.ShoppingCartItems.Remove(item);
            }
            _dbContext.SaveChanges();
        }

        public void DeleteCart(string email)
        {
            // Find user and cart
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            var cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);

            // Clear the cart and then delete it
            ClearCart(email);
            _dbContext.Remove(cart);
            _dbContext.SaveChanges();
        }
    }
}