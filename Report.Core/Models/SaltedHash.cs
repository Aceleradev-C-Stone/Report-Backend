namespace Report.Core.Models
{
    public class SaltedHash
    {
        public SaltedHash(string hash, string salt)
        {
            Hash = hash;
            Salt = salt;
        }

        public string Hash { get; set; }
        public string Salt { get; set; }
    }
}