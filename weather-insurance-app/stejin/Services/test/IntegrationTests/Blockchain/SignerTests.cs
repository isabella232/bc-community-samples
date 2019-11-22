using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherInsurance.Integration.Blockchain;

namespace IntegrationTests.Blockchain
{
    [TestClass]
    public class SignerTests
    {

        [TestMethod]
        public void SignatureTest1()
        {
            var signer = new Signer();
            var message = "Some data";

            var hash = $"0x{signer.Hash(message)}";
            Console.WriteLine($"Hash: {hash}");
            var s1 = signer.Sign("52a9fff08e03e298ce95dee54441dd00b81393b60634ccf3bb557716c3071e21", signer.ConvertToByteArray(hash));
            Console.WriteLine($"S1: {s1}");

            var s2 = signer.EncodeUTF8AndSign("52a9fff08e03e298ce95dee54441dd00b81393b60634ccf3bb557716c3071e21", message);
            Console.WriteLine($"S2: {s2}");

            Assert.IsTrue(signer.IsValid("0x59a11ed570c0b963d75cd176dcc4e0abc2584858", s2, message));
        }

        [TestMethod]
        public void SignatureTest2()
        {
            var signer = new Signer();
            var message = "1550954444168|GET|/accounts|";

            var s1 = signer.EncodeUTF8AndSign("52a9fff08e03e298ce95dee54441dd00b81393b60634ccf3bb557716c3071e21", message);
            Console.WriteLine($"S1: {s1}");

            var s2 = "0x417e703cae6f9bc67932ae8c80e58955d8abb2e98789f3a9913ef70de639bb3f316af5406af32e8a1470916a1b3c3e0600c73b35ba47313f1e5a5e9dc6c71ab01b";

            Assert.IsTrue(s1 == s2);
        }

        [TestMethod]
        public void SignatureTest3()
        {
            var signer = new Signer();
            var message = "100|GET|/contractFiles|";

            var s1 = signer.EncodeUTF8AndSign("0x852689ab676537020c5c35fba11db03754107c809a15fffa914b385612a91475", message);
            Console.WriteLine($"S1: {s1}");

            var s2 = "0x26bd346ab9077d5d35ac20b66ab2de43aa7c7962239a839ac2ced8bb3177070566d1ab2d25693235735f60e6b4c43174610f33e7825030e929d1a02433ea78871c";

            Assert.IsTrue(s1 == s2);
            Assert.IsTrue(signer.IsValid("0x7b6d3a00125e17e0a0771763a680949497b22577", s2, message));
        }

        [TestMethod]
        public void SignatureTest4()
        {
            var signer = new Signer();
            var signature = signer.EncodeUTF8AndSign("0x852689ab676537020c5c35fba11db03754107c809a15fffa914b385612a91475", "100|GET|/api/contractFiles|");
            Assert.AreEqual(signature, "0x5019a24f869216a7980937980f7e3a6bd2ec0b37284ee4bc29156113f8ba87540b0510dbed042d748f912417e333a5e93d74db392ba1bf0842da318dda2896a71c");
        }

    }
}
