namespace DevOidc2.Models.Response;

public class KeysResponseModel
{
    [JsonPropertyName("keys")]
    public KeyResponseModel[]? Keys { get; set; }
}
