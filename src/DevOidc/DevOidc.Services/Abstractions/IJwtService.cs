using System.Collections.Generic;

namespace DevOidc.Services.Abstractions
{
    // TODO: its more a provider
    public interface IJwtService
    {
        string GetPublicKey();
        string CreateJwt(Dictionary<string, object> payload);
    }
}
