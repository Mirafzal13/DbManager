namespace DbManager.Application.Services
{
    public interface IPasswordProtector
    {
        string Protect(string plainText);
        string Unprotect(string encryptedText);
    }
}
