using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.OptionsModel;

namespace Thalia.Services.Encryption
{
    /// <summary>
    /// http://stackoverflow.com/questions/273452/using-aes-encryption-in-c-sharp
    /// </summary>
    public class EncryptionService : IEncryptionService
    {
        private const int Iterations = 2;
        private const int KeySize = 256;
        private readonly IOptions<EncryptionServiceKeys> _keys;

        public EncryptionService(IOptions<EncryptionServiceKeys> keys)
        {
            _keys = keys;
        }

        public string Encrypt(string value)
        {
            var vectorBytes = Encoding.ASCII.GetBytes(_keys.Value.Vector);
            var saltBytes = Encoding.ASCII.GetBytes(_keys.Value.Salt);
            var valueBytes = Encoding.UTF8.GetBytes(value);

            byte[] encrypted;
            using (var cipher = Aes.Create())
            {
                var passwordBytes = new Rfc2898DeriveBytes(_keys.Value.Password, saltBytes, Iterations);
                var keyBytes = passwordBytes.GetBytes(KeySize / 8);

                cipher.Mode = CipherMode.CBC;

                using (var encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes))
                {
                    using (var to = new MemoryStream())
                    {
                        using (var writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write))
                        {
                            writer.Write(valueBytes, 0, valueBytes.Length);
                            writer.FlushFinalBlock();
                            encrypted = to.ToArray();
                        }
                    }
                }
                // not supported in Core 5.0?
                // cipher.Clear();
            }
            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            var vectorBytes = Encoding.ASCII.GetBytes(_keys.Value.Vector);
            var saltBytes = Encoding.ASCII.GetBytes(_keys.Value.Salt);
            var valueBytes = Convert.FromBase64String(value);

            byte[] decrypted;
            int decryptedByteCount = 0;

            using (var cipher = Aes.Create())
            {
                var passwordBytes = new Rfc2898DeriveBytes(_keys.Value.Password, saltBytes, Iterations);
                var keyBytes = passwordBytes.GetBytes(KeySize / 8);

                cipher.Mode = CipherMode.CBC;

                try
                {
                    using (var decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes))
                    {
                        using (var from = new MemoryStream(valueBytes))
                        {
                            using (var reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read))
                            {
                                decrypted = new byte[valueBytes.Length];
                                decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }

                // not supported in Core 5.0?
                //cipher.Clear();
            }

            return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
        }

        //public string Encrypt(string value)
        //{
        //    return Encrypt<AesManaged>(value, _keys.Value.Password);
        //}

        //public string Encrypt<T>(string value, string password) where T : SymmetricAlgorithm, new()
        //{
        //    var vectorBytes = Encoding.ASCII.GetBytes(_keys.Value.Vector);
        //    var saltBytes = Encoding.ASCII.GetBytes(_keys.Value.Salt);
        //    var valueBytes = Encoding.UTF8.GetBytes(value);

        //    byte[] encrypted;
        //    using (var cipher = new T())
        //    {
        //        var passwordBytes = new Rfc2898DeriveBytes(password, saltBytes, Iterations);
        //        var keyBytes = passwordBytes.GetBytes(KeySize / 8);

        //        cipher.Mode = CipherMode.CBC;

        //        using (var encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes))
        //        {
        //            using (var to = new MemoryStream())
        //            {
        //                using (var writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write))
        //                {
        //                    writer.Write(valueBytes, 0, valueBytes.Length);
        //                    writer.FlushFinalBlock();
        //                    encrypted = to.ToArray();
        //                }
        //            }
        //        }
        //        // not supported in Core 5.0?
        //        // cipher.Clear();
        //    }
        //    return Convert.ToBase64String(encrypted);
        //}

        //public string Decrypt(string value)
        //{
        //    return Decrypt<AesManaged>(value, _keys.Value.Password);
        //}
        //public string Decrypt<T>(string value, string password) where T : SymmetricAlgorithm, new()
        //{
        //    var vectorBytes = Encoding.ASCII.GetBytes(_keys.Value.Vector);
        //    var saltBytes = Encoding.ASCII.GetBytes(_keys.Value.Salt);
        //    var valueBytes = Convert.FromBase64String(value);

        //    byte[] decrypted;
        //    int decryptedByteCount = 0;

        //    using (var cipher = new T())
        //    {
        //        var passwordBytes = new Rfc2898DeriveBytes(password, saltBytes, Iterations);
        //        var keyBytes = passwordBytes.GetBytes(KeySize / 8);

        //        cipher.Mode = CipherMode.CBC;

        //        try
        //        {
        //            using (var decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes))
        //            {
        //                using (var from = new MemoryStream(valueBytes))
        //                {
        //                    using (var reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read))
        //                    {
        //                        decrypted = new byte[valueBytes.Length];
        //                        decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return string.Empty;
        //        }

        //        // not supported in Core 5.0?
        //        //cipher.Clear();
        //    }

        //    return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
        //}
    }
}
