using Newtonsoft.Json;

namespace DevOidc.Functions.Models.Response
{
    public class MetadataResponseModel
    {
        [JsonProperty("token_endpoint")]
        public string? TokenEndpoint { get; set; }

        [JsonProperty("token_endpoint_auth_methods_supported")]
        public string[]? TokenEndpointAuthMethodsSupported { get; set; }

        [JsonProperty("jwks_uri")]
        public string? JwksUri { get; set; }

        [JsonProperty("response_modes_supported")]
        public string[]? ResponseModesSupported { get; set; }

        [JsonProperty("subject_types_supported")]
        public string[]? SubjectTypesSupported { get; set; }

        [JsonProperty("id_token_signing_alg_values_supported")]
        public string[]? IdTokenSigningAlgValuesSupported { get; set; }

        [JsonProperty("response_types_supported")] 
        public string[]? ResponseTypesSupported { get; set; }

        [JsonProperty("scopes_supported")]
        public string[]? ScopesSupported { get; set; }

        [JsonProperty("issuer")]
        public string? Issuer { get; set; }

        [JsonProperty("request_uri_parameter_supported")]
        public bool RequestUriParameterSupported { get; set; }

        [JsonProperty("userinfo_endpoint")]
        public string? UserinfoEndpoint { get; set; }

        [JsonProperty("authorization_endpoint")]
        public string? AuthorizationEndpoint { get; set; }

        [JsonProperty("device_authorization_endpoint")]
        public string? DeviceAuthorizationEndpoint { get; set; }

        [JsonProperty("http_logout_supported")]
        public bool HttpLogoutSupported { get; set; }

        [JsonProperty("frontchannel_logout_supported")]
        public bool FrontchannelLogoutSupported { get; set; }

        [JsonProperty("end_session_endpoint")]
        public string? EndSessionEndpoint { get; set; }

        [JsonProperty("claims_supported")]
        public string[]? ClaimsSupported { get; set; }

        [JsonProperty("tenant_region_scope")]
        public string? TenantRegionScope { get; set; }

        [JsonProperty("cloud_instance_name")]
        public string? CloudInstanceName { get; set; }
    }
}
