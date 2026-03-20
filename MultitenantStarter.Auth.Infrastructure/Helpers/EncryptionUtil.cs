namespace MultitenantStarter.Auth.Infrastructure.Helpers.Encryption
{
    using System.Security.Cryptography;
    using System.Text;

    public static class EncryptionUtil
    {
        private const int KeySize = 256;
        private const int SaltSize = 32;
        private const int Iterations = 100000;

        public static string Encrypter(string json, string encryptSecret)
        {
            byte[] salt = GenerateSalt();
            using var aes = Aes.Create();
            using var key = new Rfc2898DeriveBytes(encryptSecret, salt, Iterations, HashAlgorithmName.SHA256);

            aes.Key = key.GetBytes(KeySize / 8);
            aes.GenerateIV();
            byte[] iv = aes.IV;

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            ms.Write(salt, 0, salt.Length);
            ms.Write(iv, 0, iv.Length);
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(json);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string DecryptText(string cipherTextBase64, string password)
        {
            byte[] fullData = Convert.FromBase64String(cipherTextBase64);

            byte[] salt = fullData[..SaltSize];
            byte[] iv = fullData[SaltSize..(SaltSize + 16)];
            byte[] cipher = fullData[(SaltSize + 16)..];

            using var aes = Aes.Create();
            using var key = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);

            aes.Key = key.GetBytes(KeySize / 8);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }

        private static byte[] GenerateSalt()
        {
            var salt = new byte[SaltSize];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return salt;
        }
    }
}
