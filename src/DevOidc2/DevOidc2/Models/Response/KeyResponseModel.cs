namespace DevOidc2.Models.Response;

public class KeyResponseModel
{
    [JsonPropertyName("kty")]
    public string? KeyType { get; set; }

    [JsonPropertyName("alg")]
    public string? Algorithm { get; set; }

    [JsonPropertyName("use")]
    public string? Use { get; set; }

    [JsonPropertyName("kid")]
    public string? Id { get; set; }

    [JsonPropertyName("n")]
    public string? Modulus { get; set; }

    [JsonPropertyName("e")]
    public string? Exponent { get; set; }

    [JsonPropertyName("issuer")]
    public string? Issuer { get; set; }
}
