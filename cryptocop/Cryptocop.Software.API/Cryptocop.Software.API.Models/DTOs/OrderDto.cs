using Cryptocop.Software.API.Models.DTOs;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cryptocop.Software.API.Models.DTOs
{
    public class OrderDto
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "fullName")]
        public string FullName { get; set; }
        [JsonProperty(PropertyName = "streetName")]
        public string StreetName { get; set; }
        [JsonProperty(PropertyName = "houseNumber")]
        public string HouseNumber { get; set; }
        [JsonProperty(PropertyName = "zipCode")]
        public string ZipCode { get; set; }
        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }
        [JsonProperty(PropertyName = "cardholderName")]
        public string CardholderName { get; set; }
        [JsonProperty(PropertyName = "creditCard")]
        public string CreditCard { get; set; }
        [JsonProperty(PropertyName = "orderDate")]
        public string OrderDate { get; set; } // Represented as 01.01.2020
        [JsonProperty(PropertyName = "totalPrice")]
        public float TotalPrice { get; set; }
        [JsonProperty(PropertyName = "orderItems")]
        public List<OrderItemDto> OrderItems { get; set; }
    }
}