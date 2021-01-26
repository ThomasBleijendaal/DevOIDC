using System.Collections.Generic;

namespace DevOidc.Business.Abstractions
{
    public interface IScopeProvider
    {
        IEnumerable<string> GetCustomScopes(IEnumerable<string> scopes);
        bool IdTokenRequested(IEnumerable<string> scopes);
        bool AccessTokenRequested(IEnumerable<string> scopes);
    }
}
