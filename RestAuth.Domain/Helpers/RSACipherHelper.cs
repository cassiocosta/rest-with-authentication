using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;

namespace RestAuth.Domain.Helpers
{
    public static class RSACipherHelper
    {
        private static string PRIVATE_PEM_CONTENT;

        private static RSAParameters privateKey;
        private static RSAParameters publicKey;

        static RSACipherHelper()
        {
            PRIVATE_PEM_CONTENT = Environment.GetEnvironmentVariable("PRIVATE_PEM_CONTENT");

            ImportKeys();
        }

        public static string EncryptString(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new InvalidParameterException("Invalid parameter data");

            var encrypted = Encrypt(data);

            return Convert.ToBase64String(encrypted);
        }

        public static bool ValidateEncryptedData(string plainData, string encryptedData)
        {
            var decripted = Decrypt(Convert.FromBase64String(encryptedData));

            return decripted.Equals(plainData);
        }

        //private static string DecryptString(string data)
        //{
        //    if (string.IsNullOrEmpty(data))
        //        throw new InvalidParameterException("Invalid parameter data");

        //    var decripted = Decrypt(Convert.FromBase64String(data));

        //    return decripted;
        //}

        private static void ImportKeys()
        {
            try
            {
                if (string.IsNullOrEmpty(PRIVATE_PEM_CONTENT))
                    throw new ApplicationException("Undefined PRIVATE_PEM_CONTENT");

                PRIVATE_PEM_CONTENT = PRIVATE_PEM_CONTENT.Replace("\\u000A", "\n");

                using var textReader = new StringReader(PRIVATE_PEM_CONTENT);
                var pemReader = new PemReader(textReader);
                var pemObject = (AsymmetricCipherKeyPair)pemReader.ReadObject();

                privateKey = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)pemObject.Private);
                publicKey = DotNetUtilities.ToRSAParameters((RsaKeyParameters)pemObject.Public);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Import keys exception", ex);
            }
        }

        private static byte[] Encrypt(string data)
        {
            byte[] encodedData = null;
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(publicKey);

                encodedData = rsa.Encrypt(dataBytes, false);
            }

            return encodedData;
        }

        private static string Decrypt(byte[] data)
        {
            byte[] decodedData = null;

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(privateKey);

                decodedData = rsa.Decrypt(data, false);
            }

            return Encoding.UTF8.GetString(decodedData);
        }
    }
}