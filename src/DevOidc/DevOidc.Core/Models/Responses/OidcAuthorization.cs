using System;
using DevOidc.Business.Abstractions.Request;

namespace DevOidc.Core.Models.Responses
{
    public class OidcAuthorization : IOidcAuthorization
    {
        public OidcAuthorization(string type, string value)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Type { get; set; }

        public string Value { get; set; }
    }
}
