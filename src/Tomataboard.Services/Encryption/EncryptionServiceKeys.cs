namespace Tomataboard.Services.Encryption
{
    public class EncryptionServiceKeys
    {
        public string Salt { get; set; }
        public string Vector { get; set; }
        public string Password { get; set; }
    }
}