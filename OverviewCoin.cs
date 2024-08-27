using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoinGecko_Asset_Tracker
{
    internal class OverviewCoin
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("current_price")]
        public double CurrentPrice { get; set; }

        [JsonProperty("market_cap")]
        public double MarketCap { get; set; }

        [JsonProperty("market_cap_rank")]
        public int MarketCapRank { get; set; }

        [JsonProperty("total_volume")]
        public double TotalVolume { get; set; }

        [JsonProperty("high_24h")]
        public double High24H { get; set; }

        [JsonProperty("low_24h")]
        public double Low24H { get; set; }

        [JsonProperty("price_change_24h")]
        public double PriceChange24H { get; set; }

        [JsonProperty("price_change_percentage_24h")]
        public double PriceChangePercentage24H { get; set; }

        [JsonProperty("circulating_supply")]
        public double CirculatingSupply { get; set; }

        [JsonProperty("total_supply")]
        public double TotalSupply { get; set; }

        [JsonProperty("ath")]
        public double Ath { get; set; }

        [JsonProperty("ath_change_percentage")]
        public double AthChangePercentage { get; set; }

        [JsonProperty("ath_date")]
        public string AthDate { get; set; }

        [JsonProperty("roi")]
        public Roi Roi { get; set; }

        [JsonProperty("last_updated")]
        public string LastUpdated { get; set; }

        [JsonProperty("sparkline_in_7d")]
        public Sparkline Sparkline { get; set; }
    }

    internal class Roi
    {
        [JsonProperty("times")]
        public double Times { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("percentage")]
        public double Percentage { get; set; }
    }

    internal class Sparkline
    {
        [JsonProperty("price")]
        public List<double> Price { get; set; }
    }
}