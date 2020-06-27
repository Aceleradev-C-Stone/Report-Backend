using Report.Infra.Services.Hash;

namespace Report.Infra.Services.Hash
{
    public interface IHashService
    {
        public SaltedHash GenerateSaltedHash(string text);
        public bool AreEqual(string text, SaltedHash saltedHash);
        public bool AreEqual(string text, string hash, string salt);
    }
}