using System;
using System.Security.Cryptography;
using System.Text;

namespace Report.Infra.Services.Hash
{
    public class HashService : IHashService
    {
        // TODO: Put inside HashConfig
        private const int ITERATIONS = 10000;
        private const int HASH_LENGTH = 24;
        private const int SALT_LENGTH = 24;
        
        public SaltedHash GenerateSaltedHash(string text)
        {
            var salt = GenerateSalt();
            var hash = GenerateHash(text, salt);
            return new SaltedHash(hash, salt);
        }

        public bool AreEqual(string text, SaltedHash saltedHash)
        {
            return AreEqual(text, saltedHash.Hash, saltedHash.Salt);
        }

        public bool AreEqual(string text, string hash, string salt)
        {
            var textHash = GenerateHash(text, salt);
            return textHash.Equals(hash);
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[SALT_LENGTH];
            RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider();
            cryptoProvider.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        private string GenerateHash(string text, string salt)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            Rfc2898DeriveBytes hasher = new Rfc2898DeriveBytes(textBytes, saltBytes, ITERATIONS);
            byte[] hash = hasher.GetBytes(HASH_LENGTH);
            return Convert.ToBase64String(hash);
        }
    }
}