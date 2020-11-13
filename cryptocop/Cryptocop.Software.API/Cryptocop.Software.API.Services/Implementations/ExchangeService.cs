using Cryptocop.Software.API.Models;
using Cryptocop.Software.API.Services.Helpers;
using Cryptocop.Software.API.Services.Interfaces;
using Cryptocop.Software.API.Models.DTOs;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System;
using AutoMapper;

namespace Cryptocop.Software.API.Services.Implementations
{
    public class ExchangeService : IExchangeService
    {
        private IMapper _mapper;

        public ExchangeService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<Envelope<ExchangeDto>> GetExchanges(int pageNumber = 1)
        {
            // Call the external API using the product identifier as an URL parameter to
            string URL = "https://data.messari.io/api/v1/markets";
            string urlParameters = "?fields=exchange_id,exchange_name,exchange_slug,quote_asset_symbol,price_usd,last_trade_at&page=" + pageNumber;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            var response = await client.GetAsync(urlParameters);
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response to a ExchangeDto model
                var exchanges = await HttpResponseMessageExtensions.DeserializeJsonToList<ExchangeDto>(response, true);

                // Create the enveloper
                var envelope = new Envelope<ExchangeDto>
                {
                    Items = exchanges,
                    PageNumber = pageNumber
                };

                client.Dispose();

                return envelope;
            }
            else
            {
                throw new Exception("Request failed to external API.");
            }
        }
    }
}