using Newtonsoft.Json;

namespace Cryptocop.Software.API.Models.DTOs
{
    public class OrderItemDto
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "productIdentifier")]
        public string ProductIdentifier { get; set; }
        [JsonProperty(PropertyName = "quantity")]
        public float Quantity { get; set; }
        [JsonProperty(PropertyName = "unitPrice")]
        public float UnitPrice { get; set; }
        [JsonProperty(PropertyName = "totalPrice")]
        public float TotalPrice { get; set; }
    }
}