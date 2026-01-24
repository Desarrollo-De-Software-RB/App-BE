using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Encryption;

namespace TvTracker.Security
{
    public class PlainStringEncryptionService : IStringEncryptionService
    {
        public string Encrypt(string plainText, string passPhrase = null, byte[] salt = null)
        {
            return plainText;
        }

        public string Decrypt(string cipherText, string passPhrase = null, byte[] salt = null)
        {
            return cipherText;
        }
    }
}
