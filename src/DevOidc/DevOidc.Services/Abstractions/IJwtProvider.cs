using System;
using System.Collections.Generic;

namespace DevOidc.Services.Abstractions
{
    public interface IJwtProvider
    {
        string CreateJwt(Dictionary<string, object> claims, TimeSpan lifetime, IEncryptionProvider encryptionProvider);
    }
}
