using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Cryptography;

namespace InscriptionsApi.Controllers.Encryption
{
    public class HashEncryption
    {
        public static HashedFormat Hash(string password)
        {

            byte[] hashAlgorithm = new byte[128 / 8];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(hashAlgorithm);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt:hashAlgorithm,
                prf:KeyDerivationPrf.HMACSHA256,
                iterationCount: 11000,
                numBytesRequested:256/8));

            return new HashedFormat() { Password = hashed, HashAlgorithm= Convert.ToBase64String(hashAlgorithm) };
        }
        
        public static bool CheckHash(string password, string hashToCompare, string hashAlgorithm)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String( hashAlgorithm),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 11000,
                numBytesRequested: 256 / 8));
            return hashed == hashToCompare;
        }

        
    }
}
