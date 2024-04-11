using Microsoft.AspNetCore.DataProtection;

namespace KittyShop.Utility
{
    public class CipherService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IConfiguration _configuration;

        public CipherService(IDataProtectionProvider dataProtectionProvider, IConfiguration configuration)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _configuration = configuration;
        }
        public string Encrypt(string input)
        {
            var key = _configuration.GetSection("Encryption")["EncryptionKey"];
            var protector = _dataProtectionProvider.CreateProtector(key);
            return protector.Protect(input);
        }
        public string Decrypt(string cipherText)
        {
            var key = _configuration.GetSection("Encryption")["EncryptionKey"];
            var protector = _dataProtectionProvider.CreateProtector(key);
            return protector.Unprotect(cipherText);
        }
    }
}
