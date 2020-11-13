using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Cryptocop.Software.API.Services.Helpers;
using Cryptocop.Software.API.Services.Interfaces;
using Cryptocop.Software.API.Models.DTOs;
using System;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using AutoMapper;

namespace Cryptocop.Software.API.Services.Implementations
{
    public class CryptoCurrencyService : ICryptoCurrencyService
    {
        private IMapper _mapper;

        public CryptoCurrencyService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<CryptoCurrencyDto>> GetAvailableCryptocurrencies()
        {
            // Call the external API using the product identifier as an URL parameter to
            string URL = "https://data.messari.io/api/v2/assets";
            string urlParameters = "?fields=id,symbol,name,slug,metrics/market_data/price_usd,profile/general/overview/project_details";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            var response = await client.GetAsync(urlParameters);
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response to a CryptoCurrencyDto model
                var cryptoCurrencies = await HttpResponseMessageExtensions.DeserializeJsonToList<CryptoCurrencyDto>(response, true);

                // Filter the list for BTC, ETH, USDT and XMR
                var filteredCryptoCurrencies = cryptoCurrencies.Where(c => c.Symbol == "BTC" || c.Symbol == "ETH" || c.Symbol == "USDT" || c.Symbol == "XMR").ToList();

                client.Dispose();

                return filteredCryptoCurrencies;
            }
            else
            {
                throw new Exception("Request failed.");
            }
        }
    }
}