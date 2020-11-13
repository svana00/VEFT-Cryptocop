using Cryptocop.Software.API.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Cryptocop.Software.API.Repositories.Interfaces;
using Cryptocop.Software.API.Services.Helpers;
using Cryptocop.Software.API.Models.DTOs;
using Cryptocop.Software.API.Models.InputModels;
using System.Net.Http.Headers;
using System;

namespace Cryptocop.Software.API.Services.Implementations
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public ShoppingCartService(IShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }

        public IEnumerable<ShoppingCartItemDto> GetCartItems(string email)
        {
            return _shoppingCartRepository.GetCartItems(email);
        }

        public async Task AddCartItem(string email, ShoppingCartItemInputModel shoppingCartItem)
        {
            // Call the external API using the product identifier as an URL parameter to
            string URL = "https://data.messari.io/api/v1/assets/BTC/metrics";
            string urlParameters = "?fields=id,symbol,name,slug,market_data/price_usd";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            var response = await client.GetAsync(urlParameters);
            if (response.IsSuccessStatusCode)
            {
                // Receive the current price in USD for this particular cryptocurrency
                // Deserialize the response to a CryptoCurrencyDto model
                var responseObject = await HttpResponseMessageExtensions.DeserializeJsonToObject<CryptoCurrencyDto>(response, true);

                client.Dispose();
                _shoppingCartRepository.AddCartItem(email, shoppingCartItem, responseObject.PriceInUsd);
            }
            else
            {
                throw new Exception("Response failed to external API.");
            }
        }

        public void RemoveCartItem(string email, int id)
        {
            _shoppingCartRepository.RemoveCartItem(email, id);
        }

        public void UpdateCartItemQuantity(string email, int id, float quantity)
        {
            _shoppingCartRepository.UpdateCartItemQuantity(email, id, quantity);
        }

        public void ClearCart(string email)
        {
            _shoppingCartRepository.ClearCart(email);
        }
    }
}
