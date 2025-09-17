using Microsoft.AspNetCore.DataProtection;

namespace DbManager.Application.Services
{
    public class PasswordProtector : IPasswordProtector
    {
        private readonly IDataProtector _protector;

        public PasswordProtector(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("ConnectionStringsProtector");
        }

        public string Protect(string plainText) => _protector.Protect(plainText);

        public string Unprotect(string encryptedText) => _protector.Unprotect(encryptedText);
    }
}
