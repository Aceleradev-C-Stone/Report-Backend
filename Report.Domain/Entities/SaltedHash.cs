using System;
using System.Security.Cryptography;
using System.Text;

namespace Report.Domain.Entities
{
    public class SaltedHash
    {
        // TODO: Put inside HashConfig
        private const int ITERATIONS = 10000;
        private const int HASH_LENGTH = 24;
        private const int SALT_LENGTH = 24;

        public static SaltedHash Generate(string text)
        {
            return new SaltedHash().GenerateSaltedHash(text);
        }

        public static bool AreEqual(string text, string hash, string salt)
        {
            var saltedHash = new SaltedHash(hash, salt);
            return saltedHash.Compare(text);
        }

        private SaltedHash() {}

        private SaltedHash(string hash, string salt)
        {
            Hash = hash;
            Salt = salt;
        }

        public bool Compare(string text)
        {
            string textHash = GenerateHash(text, Salt);
            return Hash.Equals(textHash);
        }

        private SaltedHash GenerateSaltedHash(string text)
        {
            Salt = GenerateSalt();
            Hash = GenerateHash(text, Salt);
            return this;
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

        public string Salt { get; private set; }
        public string Hash { get; private set; }
    }
}