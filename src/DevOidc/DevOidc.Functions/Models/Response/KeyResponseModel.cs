using Newtonsoft.Json;

namespace DevOidc.Functions.Models.Response
{
    public class KeyResponseModel
    {
        [JsonProperty("kty")]
        public string? Kty { get; set; }

        [JsonProperty("alg")]
        public string? Alg { get; set; }

        [JsonProperty("use")]
        public string? Use { get; set; }

        [JsonProperty("kid")]
        public string? Kid { get; set; }

        [JsonProperty("x5t")]
        public string? X5t { get; set; }

        [JsonProperty("n")]
        public string? N { get; set; }

        [JsonProperty("e")]
        public string? E { get; set; }

        [JsonProperty("x5c")]
        public string[]? X5c { get; set; }

        [JsonProperty("issuer")]
        public string? Issuer { get; set; }
    }

}
