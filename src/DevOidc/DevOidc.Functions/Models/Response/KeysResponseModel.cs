using Newtonsoft.Json;

namespace DevOidc.Functions.Models.Response
{
    public class KeysResponseModel
    {
        [JsonProperty("keys")]
        public KeyResponseModel[]? Keys { get; set; }
    }

}
