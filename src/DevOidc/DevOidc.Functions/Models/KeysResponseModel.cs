using Newtonsoft.Json;

namespace DevOidc.Functions.Models
{
    public class KeysResponseModel
    {
        [JsonProperty("keys")]
        public KeyResponseModel[]? Keys { get; set; }
    }

}
