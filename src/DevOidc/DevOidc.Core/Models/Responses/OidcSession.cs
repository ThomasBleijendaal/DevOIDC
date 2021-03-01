using System;
using DevOidc.Business.Abstractions.Request;

namespace DevOidc.Core.Models.Responses
{
    public class OidcSession : IOidcSession
    {
        public OidcSession(string code)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        public string Code { get; set; }
    }
}
