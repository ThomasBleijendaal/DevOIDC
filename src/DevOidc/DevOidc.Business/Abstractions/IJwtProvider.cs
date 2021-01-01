using System;
using System.Collections.Generic;

namespace DevOidc.Business.Abstractions
{
    public interface IJwtProvider
    {
        string CreateJwt(Dictionary<string, object> claims, TimeSpan lifetime, IEncryptionProvider encryptionProvider);
    }
}
