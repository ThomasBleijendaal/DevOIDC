using System;
using DevOidc.Core.Models;

namespace DevOidc.Services.Abstractions
{
    public interface IEncryptionProvider
    {
        string GetKeyId();

        KeyDto GetPublicKey();

        IDisposable CreateKey();
    }
}
