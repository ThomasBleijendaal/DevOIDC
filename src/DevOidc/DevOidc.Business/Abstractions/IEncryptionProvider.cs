using System;
using DevOidc.Core.Models.Dtos;

namespace DevOidc.Business.Abstractions
{
    public interface IEncryptionProvider
    {
        string GetKeyId();

        KeyDto GetPublicKey();

        IDisposable CreateKey();
    }
}
