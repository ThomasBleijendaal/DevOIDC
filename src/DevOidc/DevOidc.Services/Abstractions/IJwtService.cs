using System.Collections.Generic;
using DevOidc.Core.Models;

namespace DevOidc.Services.Abstractions
{
    // TODO: its more a provider
    public interface IJwtService
    {
        KeyDto GetPublicKey();
        string CreateJwt(Dictionary<string, object> payload);
    }
}
