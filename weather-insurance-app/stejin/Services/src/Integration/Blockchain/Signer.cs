using System;
using Microsoft.Extensions.Logging;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;

namespace WeatherInsurance.Integration.Blockchain
{
    public class Signer
    {
        EthereumMessageSigner _signer;

        public Signer()
        {
            _signer = new EthereumMessageSigner();
        }

        public string Hash(string message)
        {
            var hasher = new Nethereum.Util.Sha3Keccack();
            var hash = hasher.CalculateHash(message);
            return hash;
        }

        public byte[] ConvertToByteArray(string hash)
        {
            return hash.HexToByteArray();
        }

        public string HashAndSign(string privateKey, string message)
        {
            return _signer.HashAndSign(message, privateKey);
        }

        public string Sign(string privateKey, byte[] message)
        {
            return _signer.Sign(message, privateKey);
        }

        public string EncodeUTF8AndSign(string privateKey, string message)
        {
            var key = new EthECKey(privateKey);
            return _signer.EncodeUTF8AndSign(message, key);
        }

        public bool IsValid(string address, string signature, string message)
        {
            var r = _signer.EncodeUTF8AndEcRecover(message, signature);
            return address.ToLower() == r.ToLower();
        }

        /// <summary>
        /// Verifies a signature given a publicKey and a message.
        /// </summary>
        /// <param name="logger">A logger implementation.</param>
        /// <returns>The result of the signature check.</returns>
        public static Func<string, string, string, bool> VerifySignature(ILogger<Signer> logger = null)
        {
            return (signature, publicKey, message) =>
            {
                try
                {
                    var signer = new Signer();
                    var isValid = signer.IsValid(publicKey, signature, message);
                    return isValid;
                }
                catch (Exception ex)
                {
                    if (logger != null)
                        logger.LogError($"{ex.Message}{Environment.NewLine}Signature: {signature}, Key: {publicKey}, Message: {message}");
                    return false;
                }
            };
        }

    }

}
