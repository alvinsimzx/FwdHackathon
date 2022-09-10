using Newtonsoft.Json;

namespace FwdHackathon.Models
{
    public class TrendsList
    {
        [JsonProperty("trends")]
        public List<Trend> trends { get; set; }
    }
}
