//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using DevOidc.Core.Models;
//using DevOidc.Services.Abstractions;
//using Microsoft.IdentityModel.Tokens;
//using Newtonsoft.Json;

//namespace DevOidc.Services.Jwt
//{
//    public class HS256JwtService : IJwtService
//    {
//        public string CreateJwt(Dictionary<string, object> payload)
//        {
//            var localPayload = payload.ToDictionary(x => x.Key, x => x.Value);
//            localPayload.Add("exp", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

//            var header = new
//            {
//                typ = "JWT",
//                alg = "HS256",
//                kid = "default-kid"
//            };

//            var headerBase64 = Base64UrlEncoder.Encode(JsonConvert.SerializeObject(header));

//            var payloadJson = JsonConvert.SerializeObject(localPayload);

//            var payloadBase64 = Base64UrlEncoder.Encode(payloadJson);

//            var payloadJsonBytes = Encoding.UTF8.GetBytes($"{headerBase64}.{payloadBase64}");

//            var signatureBytes = HashHMAC(Encoding.UTF8.GetBytes("super-secret"), payloadJsonBytes);

//            var signature = Base64UrlEncoder.Encode(signatureBytes);

//            return $"{headerBase64}.{payloadBase64}.{signature}";
//        }

//        public KeyDto GetPublicKey() => new KeyDto();

//        private byte[] HashHMAC(byte[] key, byte[] message)
//        {
//            using var hash = new HMACSHA256(key);
//            return hash.ComputeHash(message);
//        }
//    }
//}
