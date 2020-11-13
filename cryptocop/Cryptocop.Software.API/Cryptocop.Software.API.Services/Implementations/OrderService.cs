using System.Collections.Generic;
using Cryptocop.Software.API.Repositories.Interfaces;
using Cryptocop.Software.API.Services.Interfaces;
using Cryptocop.Software.API.Models.DTOs;
using Cryptocop.Software.API.Models.InputModels;
using Microsoft.Extensions.Configuration;

namespace Cryptocop.Software.API.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IQueueService _queueService;
        private readonly IOrderRepository _orderRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly string _routingKey;

        public OrderService(IConfiguration configuration, IQueueService queueService, IOrderRepository orderRepository, IShoppingCartRepository shoppingCartRepository)
        {
            _orderRepository = orderRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _queueService = queueService;
            _routingKey = configuration.GetSection("MessageBroker").GetSection("RoutingKey").Value;
        }

        public IEnumerable<OrderDto> GetOrders(string email)
        {
            return _orderRepository.GetOrders(email);
        }

        public void CreateNewOrder(string email, OrderInputModel order)
        {
            var newOrder = _orderRepository.CreateNewOrder(email, order);
            _shoppingCartRepository.DeleteCart(email);
            _queueService.PublishMessage(_routingKey, newOrder);
        }
    }
}