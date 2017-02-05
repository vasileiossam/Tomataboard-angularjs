namespace Tomataboard.Services.Encryption
{
    public interface IEncryptionService
    {
        string Encrypt(string value);

        string Decrypt(string value);
    }
}