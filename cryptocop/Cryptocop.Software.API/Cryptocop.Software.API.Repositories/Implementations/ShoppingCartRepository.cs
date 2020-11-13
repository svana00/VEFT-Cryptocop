using System.Collections.Generic;
using Cryptocop.Software.API.Repositories.Interfaces;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Models.DTOs;
using Cryptocop.Software.API.Repositories.Contexts;
using System.Linq;
using System;
using AutoMapper;
using Cryptocop.Software.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Cryptocop.Software.API.Models.Exceptions;

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
            // Find the user
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                throw new Exception("User not found.");
            };

            // Find shopping cart
            var cart = _dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == user.Id);

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
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) { throw new ResourceNotFoundException("User not found."); }
            var cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);

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
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            var cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);

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
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            var cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);

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
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            var cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);

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

            var itemsToDelete = _dbContext.ShoppingCartItems.Where(s => s.ShoppingCartId == cart.Id);
            foreach (var item in itemsToDelete)
            {
                _dbContext.ShoppingCartItems.Remove(item);
            }
            _dbContext.SaveChanges();
        }

        public void DeleteCart(string email)
        {
            // Find user
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            var cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);
            ClearCart(email);
            _dbContext.Remove(cart);
            _dbContext.SaveChanges();
        }
    }
}