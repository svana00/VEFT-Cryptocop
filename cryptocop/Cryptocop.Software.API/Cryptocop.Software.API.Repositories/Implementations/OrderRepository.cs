using Cryptocop.Software.API.Repositories.Interfaces;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Models.DTOs;
using Cryptocop.Software.API.Repositories.Contexts;
using Cryptocop.Software.API.Repositories.Helpers;
using Cryptocop.Software.API.Models.Entities;
using Cryptocop.Software.API.Models.Exceptions;
using System;
using System.Linq;
using AutoMapper;
using System.Collections.Generic;

namespace Cryptocop.Software.API.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CryptocopDbContext _dbContext;
        private IMapper _mapper;

        public OrderRepository(CryptocopDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<OrderDto> GetOrders(string email)
        {
            // Find user
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            var orders = _dbContext.Orders.Where(o => o.UserId == user.Id).ToList();

            // Map orders to dto
            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);

            // Find orderItems and insert into dto
            foreach (var orderDto in ordersDto)
            {
                var orderItems = _dbContext.OrderItems.Where(o => o.OrderId == orderDto.Id).ToList();
                var orderItemsDtoList = _mapper.Map<List<OrderItemDto>>(orderItems);
                orderDto.OrderItems = orderItemsDtoList;
            }

            return ordersDto;
        }

        public OrderDto CreateNewOrder(string email, OrderInputModel order)
        {
            // Find user
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            // Find address
            var address = _dbContext.Addresses.FirstOrDefault(a => a.UserId == user.Id && a.Id == order.AddressId);

            if (address == null)
            {
                throw new ResourceNotFoundException($"Address with id {order.AddressId} not found.");
            }

            // Find payment card
            var paymentCard = _dbContext.PaymentCards.FirstOrDefault(p => p.UserId == user.Id && p.Id == order.PaymentCardId);

            if (paymentCard == null)
            {
                throw new ResourceNotFoundException($"Payment card with id {order.PaymentCardId} not found.");
            }

            var cart = _dbContext.ShoppingCarts.FirstOrDefault(s => s.UserId == user.Id);

            if (cart == null)
            {
                throw new Exception("Shopping cart is empty.");
            }

            var cartItems = _dbContext.ShoppingCartItems.Where(s => s.ShoppingCartId == cart.Id);

            if (!cartItems.Any())
            {
                throw new Exception("Shopping cart is empty.");
            }

            // Find the total price
            var totalPrice = 0.0;
            foreach (var cartItem in cartItems)
            {
                totalPrice += cartItem.Quantity * cartItem.UnitPrice;
            }

            // Mask the credit card with helper function
            var maskedCreditCard = PaymentCardHelper.MaskPaymentCard(paymentCard.CardNumber);

            // Add the order to the database
            var newOrder = new Order
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                StreetName = address.StreetName,
                HouseNumber = address.HouseNumber,
                ZipCode = address.ZipCode,
                Country = address.Country,
                City = address.City,
                CardholderName = paymentCard.CardholderName,
                MaskedCreditCard = maskedCreditCard,
                OrderDate = DateTime.Now,
                TotalPrice = (float)totalPrice,
            };
            _dbContext.Orders.Add(newOrder);
            _dbContext.SaveChanges();

            // Map cart items to order items
            foreach (var cartItem in cartItems)
            {
                var newOrderItem = new OrderItem
                {
                    ProductIdentifier = cartItem.ProductIdentifier,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    TotalPrice = cartItem.Quantity * cartItem.UnitPrice,
                    OrderId = newOrder.Id
                };

                _dbContext.OrderItems.Add(newOrderItem);
            }
            _dbContext.SaveChanges();

            var orderItems = _dbContext.OrderItems.Where(o => o.OrderId == newOrder.Id).ToList();

            // Map order items to dto
            var orderItemsDtoList = _mapper.Map<List<OrderItemDto>>(orderItems);

            // Map order to dto and return
            var newOrderDto = new OrderDto
            {
                Id = newOrder.Id,
                Email = user.Email,
                FullName = user.FullName,
                StreetName = address.StreetName,
                HouseNumber = address.HouseNumber,
                ZipCode = address.ZipCode,
                Country = address.Country,
                City = address.City,
                CardholderName = paymentCard.CardholderName,
                CreditCard = paymentCard.CardNumber,
                OrderDate = newOrder.OrderDate.ToString("MM.dd.yyyy"),
                TotalPrice = (float)totalPrice,
                OrderItems = orderItemsDtoList
            };

            return newOrderDto;
        }
    }
}