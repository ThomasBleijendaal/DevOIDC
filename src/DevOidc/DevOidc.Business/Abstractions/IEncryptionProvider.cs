using System;
using DevOidc.Core.Models;

namespace DevOidc.Business.Abstractions
{
    public interface IEncryptionProvider
    {
        string GetKeyId();

        KeyDto GetPublicKey();

        IDisposable CreateKey();
    }
}
