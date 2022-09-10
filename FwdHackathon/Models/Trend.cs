using Newtonsoft.Json;

namespace FwdHackathon.Models
{
    public class Trend
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("promoted_content")]
        public string promoted_content { get; set; }

        [JsonProperty("query")]
        public string query { get; set; }

        [JsonProperty("tweet_volume")]
        public string tweet_volume { get; set; }
    }
}
