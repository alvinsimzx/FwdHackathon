using Newtonsoft.Json;

namespace FwdHackathon.Models
{
  public class Category
  {
    [JsonProperty("key")]
    public string key { get; set; }
    [JsonProperty("value")]
    public int value { get; set; }
  }
}
