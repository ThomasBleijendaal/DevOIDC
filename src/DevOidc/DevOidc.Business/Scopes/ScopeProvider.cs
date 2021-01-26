using System.Collections.Generic;
using System.Linq;
using DevOidc.Business.Abstractions;

namespace DevOidc.Business.Scopes
{
    public class ScopeProvider : IScopeProvider
    {
        public bool AccessTokenRequested(IEnumerable<string> scopes)
            => true; // GetCustomScopes(scopes).Any() || scopes.Contains("offline_access");

        public IEnumerable<string> GetCustomScopes(IEnumerable<string> scopes)
            => scopes.Except(new[] { "openid", "offline_access", "profile" });

        public bool IdTokenRequested(IEnumerable<string> scopes)
            => !GetCustomScopes(scopes.Except(new[] { "offline_access" })).Any();
    }
}
