using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DevOidc.Business.Abstractions;
using DevOidc.Core.Models;
using Jose;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace DevOidc.Business.Providers
{
    public class RS256EncryptionProvider : IEncryptionProvider
    {
        private readonly string _publicKey;
        private readonly string _keyId;
        private readonly string _privateKey;

        public RS256EncryptionProvider(
            string publicKey,
            string privateKey)
        {
            using var sha1 = SHA1.Create();
            _publicKey = publicKey;
            _keyId = Base64Url.Encode(sha1.ComputeHash(Encoding.UTF8.GetBytes(_publicKey)));
            _privateKey = privateKey;
        }

        public KeyDto GetPublicKey()
        {
            using var publicKey = new StringReader(_publicKey);

            var ppemReader = new PemReader(publicKey);

            if (ppemReader.ReadObject() is RsaKeyParameters rkp)
            {
                return new KeyDto
                {
                    Algorithm = "RS256",
                    Exponent = Convert.ToBase64String(rkp.Exponent.ToByteArrayUnsigned()),
                    KeyType = "RSA",
                    Id = _keyId,
                    Modulus = Convert.ToBase64String(rkp.Modulus.ToByteArrayUnsigned()),
                    Use = "sig"
                };
            }
            else
            {
                return new KeyDto();
            }
        }

        public IDisposable CreateKey()
        {
            using var privateKey = new StringReader(_privateKey);

            var pemReader = new PemReader(privateKey);

            if (pemReader.ReadObject() is not AsymmetricCipherKeyPair keyPair)
            {
                throw new InvalidOperationException("Provided keys are invalid");
            }

            var privateRsaParams = keyPair.Private as RsaPrivateCrtKeyParameters;
            var rsaParams = DotNetUtilities.ToRSAParameters(privateRsaParams);

            var rsa = new RSACryptoServiceProvider();

            rsa.ImportParameters(rsaParams);

            return rsa;
        }

        public string GetKeyId()
        {
            return _keyId;
        }
    }
}
