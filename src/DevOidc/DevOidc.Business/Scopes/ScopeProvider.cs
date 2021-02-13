using System.Collections.Generic;
using System.Linq;
using DevOidc.Business.Abstractions;
using DevOidc.Core.Models;

namespace DevOidc.Business.Scopes
{
    public class ScopeProvider : IScopeProvider
    {
        public bool AccessTokenRequested(SessionDto session)
            => true; // GetCustomScopes(scopes).Any() || scopes.Contains("offline_access"); // TODO: audience

        public IEnumerable<string> GetCustomScopes(IEnumerable<string> scopes)
            => scopes.Except(new[] { "openid", "offline_access", "profile" });

        public bool IdTokenRequested(SessionDto session)
            => true; // GetCustomScopes(scopes.Except(new[] { "offline_access" })).Any();
    }
}
