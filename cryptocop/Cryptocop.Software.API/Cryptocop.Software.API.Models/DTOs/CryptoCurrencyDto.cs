using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cryptocop.Software.API.Models.DTOs
{
    public class CryptoCurrencyDto
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Symbol { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Slug { get; set; }

        [DataMember]
        public float PriceInUsd
        {
            get
            {
                return _priceUsd;
            }
            set
            {
                _priceUsd = value;
            }
        }

        [DataMember]
        [JsonProperty("price_usd")]
        private float _priceUsd { get; set; }

        [DataMember]
        public string ProjectDetails
        {
            get
            {
                return _projectDetails;
            }
            set
            {
                _projectDetails = value;
            }
        }

        [DataMember]
        [JsonProperty("project_details")]
        private string _projectDetails { get; set; }

        public bool ShouldSerialize_projectDetails()
        {
            return false;
        }

        public bool ShouldSerialize_priceUsd()
        {
            return false;
        }
    }
}