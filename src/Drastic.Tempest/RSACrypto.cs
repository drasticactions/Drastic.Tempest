using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Tempest
{
    public class RSACrypto
            : IAsymmetricCrypto
    {
        private readonly int keySize;

        public RSACrypto()
            : this(2048)
        {
        }

        public RSACrypto(int keySize)
        {
            this.keySize = keySize;
            List<string> nalgs = new List<string>();

            try
            {
                if (CryptoConfig.CreateFromName("SHA256") != null)
                    nalgs.Add("SHA256");
            }
            catch
            {
            }

            try
            {
                if (CryptoConfig.CreateFromName("SHA1") != null)
                    nalgs.Add("SHA1");
            }
            catch
            {
            }

            this.rsaCrypto = new RSACryptoServiceProvider(keySize);
            this.algs = nalgs;
        }

        public IEnumerable<string> SupportedHashAlgs
        {
            get { return this.algs; }
        }

        public int KeySize
        {
            get { return this.keySize; }
        }

        public byte[] Encrypt(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            return this.rsaCrypto.Encrypt(data, true);
        }

        public byte[] Decrypt(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            return this.rsaCrypto.Decrypt(data, true);
        }

        public byte[] HashAndSign(string hashAlg, byte[] data, int offset, int count)
        {
            if (hashAlg == null)
                throw new ArgumentNullException("hashAlg");
            if (data == null)
                throw new ArgumentNullException("data");

            var hasher = CryptoConfig.CreateFromName(hashAlg) as HashAlgorithm;

            if (hasher == null)
                throw new ArgumentException("Hash algorithm not found", "hashAlg");

            return this.rsaCrypto.SignData(data, offset, count, hashAlg);
        }

        public bool VerifySignedHash(string hashAlg, byte[] data, byte[] signature)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (signature == null)
                throw new ArgumentNullException("signature");

            return this.rsaCrypto.VerifyData(data, hashAlg, signature);
        }

        public RSAAsymmetricKey ExportKey(bool includePrivate)
        {
            return new RSAAsymmetricKey(this.rsaCrypto.ExportParameters(includePrivate));
        }

        public void ImportKey(RSAAsymmetricKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            var rsaKey = (key as RSAAsymmetricKey);
            if (rsaKey == null)
                throw new ArgumentException("key must be RSAAsymmetricKey");

            this.rsaCrypto.ImportParameters(rsaKey);
        }

        private readonly RSACryptoServiceProvider rsaCrypto;
        private readonly IEnumerable<string> algs;
    }
}
