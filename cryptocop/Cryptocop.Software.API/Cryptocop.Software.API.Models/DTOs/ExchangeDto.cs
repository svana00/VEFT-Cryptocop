using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Cryptocop.Software.API.Models.DTOs
{
    [DataContract]
    public class ExchangeDto
    {
        [DataMember]
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        [DataMember]
        [JsonProperty("exchange_id")]
        private string _id { get; set; }

        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        [DataMember]
        [JsonProperty("exchange_name")]
        private string _name { get; set; }

        [DataMember]
        public string Slug
        {
            get
            {
                return _slug;
            }
            set
            {
                _slug = value;
            }
        }
        [DataMember]
        [JsonProperty("exchange_slug")]
        private string _slug { get; set; }

        [DataMember]
        public string AssetSymbol
        {
            get
            {
                return _assetSymbol;
            }
            set
            {
                _assetSymbol = value;
            }
        }

        [DataMember]
        [JsonProperty("quote_asset_symbol")]
        private string _assetSymbol { get; set; }

        [DataMember]
        public Nullable<float> PriceInUsd
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
        private Nullable<float> _priceUsd { get; set; }

        [DataMember]
        public Nullable<DateTime> LastTrade
        {
            get
            {
                return _lastTrade;
            }
            set
            {
                _lastTrade = value;
            }
        }

        [DataMember]
        [JsonProperty("last_trade_at")]
        private Nullable<DateTime> _lastTrade { get; set; }

        public bool ShouldSerialize_id()
        {
            return false;
        }

        public bool ShouldSerialize_name()
        {
            return false;
        }

        public bool ShouldSerialize_slug()
        {
            return false;
        }

        public bool ShouldSerialize_assetSymbol()
        {
            return false;
        }

        public bool ShouldSerialize_priceUsd()
        {
            return false;
        }

        public bool ShouldSerialize_lastTrade()
        {
            return false;
        }
    }
}