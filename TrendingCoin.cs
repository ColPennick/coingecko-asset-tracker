using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoinGecko_Asset_Tracker
{
    internal class TrendingCoin
    {
        [JsonProperty("item")]
        public TrendingCoinItem Item { get; set; }
    }

    internal class TrendingCoinItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("coin_id")]
        public int CoinId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("market_cap_rank")]
        public int MarketCapRank { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("price_btc")]
        public double PriceBtc { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("price_change_percentage_24h")]
        public Dictionary<string, double> PriceChangePercentage24H { get; set; }

        [JsonProperty("market_cap")]
        public string MarketCap { get; set; }

        [JsonProperty("total_volume")]
        public string TotalVolume { get; set; }

        [JsonProperty("sparkline")]
        public string Sparkline { get; set; }

        [JsonProperty("thumb")]
        public string IconMedium { get; set; }

        [JsonProperty("small")]
        public string IconSmall { get; set; }

        [JsonProperty("large")]
        public string IconLarge { get; set; }
    }

    internal class TrendingCoinsResponse
    {
        [JsonProperty("coins")]
        public List<TrendingCoin> Coins { get; set; }
    }

}

