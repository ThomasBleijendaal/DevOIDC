using System;
using System.Collections.Generic;
using System.Linq;
using DevOidc.Services.Abstractions;
using Jose;

namespace DevOidc.Services.Providers
{
    public class RS256JwtProvider : IJwtProvider
    {
        public string CreateJwt(Dictionary<string, object> claims, TimeSpan lifetime, IEncryptionProvider encryptionProvider)
        {
            var localClaims = claims.ToDictionary(x => x.Key, x => x.Value);

            localClaims.Add("exp", DateTimeOffset.UtcNow.Add(lifetime).ToUnixTimeSeconds());
            localClaims.Add("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            using var key = encryptionProvider.CreateKey();
            return JWT.Encode(localClaims, key, JwsAlgorithm.RS256, extraHeaders: new Dictionary<string, object>
            {
                { "typ", "JWT" },
                { "kid", encryptionProvider.GetKeyId() }
            });
        }
    }
}
