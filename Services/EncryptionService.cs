using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EncryptionApi.Services
{
    public class EncryptionService
    {
        // ¡IMPORTANTE! Estas claves y IV deben ser manejadas de forma segura.
        // Para este ejemplo, están hardcodeadas, pero en producción deberías.
        // generarlas aleatoriamente y almacenarlas de forma segura (ej. Azure Key Vault, HashiCorp Vault, o pasarlas de forma segura al desencriptador).
        // La clave y el IV deben ser los mismos para encriptar y desencriptar.

        // EJEMPLO DE CADENA DE 32 CARACTERES: (Asegúrate de que tu compañero use la misma)
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");

        // EJEMPLO DE CADENA DE 16 CARACTERES: (Asegúrate de que tu compañero use la misma)
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("VectorInicial16C"); // ¡Esta tiene 16 caracteres!

        public byte[] EncryptFile(byte[] fileData)
        {
            if (Key.Length != 32) throw new ArgumentException("Key must be 32 bytes (256 bits) for AES-256. Current length: " + Key.Length);
            if (IV.Length != 16) throw new ArgumentException("IV must be 16 bytes (128 bits) for AES. Current length: " + IV.Length);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(fileData, 0, fileData.Length);
                        csEncrypt.FlushFinalBlock(); // Asegura que todos los datos se escriban
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        public byte[] DecryptFile(byte[] cipherData)
        {
            if (Key.Length != 32) throw new ArgumentException("Key must be 32 bytes (256 bits) for AES-256. Current length: " + Key.Length);
            if (IV.Length != 16) throw new ArgumentException("IV must be 16 bytes (128 bits) for AES. Current length: " + IV.Length);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(cipherData, 0, cipherData.Length);
                        csDecrypt.FlushFinalBlock(); // Asegura que todos los datos se escriban
                    }
                    return msDecrypt.ToArray();
                }
            }
        }
    }
}
