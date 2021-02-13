using System.Collections.Generic;
using DevOidc.Core.Models;

namespace DevOidc.Business.Abstractions
{
    public interface IScopeProvider
    {
        IEnumerable<string> GetCustomScopes(IEnumerable<string> scopes);
        bool IdTokenRequested(SessionDto session);
        bool AccessTokenRequested(SessionDto session);
    }
}
