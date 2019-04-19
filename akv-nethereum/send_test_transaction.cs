using System;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;

namespace akv_nethereum
{
    public static class send_test_transaction
    {
        [FunctionName("send_test_transaction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            try
            {
                // Load application settings
                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                // Retrieve secrets from Key Vault
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVault =
                    new KeyVaultClient(
                        new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                var keystoreSecretBundle = await keyVault.GetSecretAsync(config["KEYSTORE_SECRET_ID"]);
                var passphraseSecretBundle = await keyVault.GetSecretAsync(config["PASSPHRASE_SECRET_ID"]);

                // Sign and send a zero-value self transaction
                var web3 = new Web3(config.GetValue<string>("ETH_JSON_RPC"));
                var account = Account.LoadFromKeyStore(keystoreSecretBundle.Value, passphraseSecretBundle.Value);
                var to = account.Address;
                var amount = new BigInteger(0);
                var nonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address);
                var gasPrice = BigInteger.Zero;
                var gasLimit = new BigInteger(21000);
                var signedTransaction = Web3.OfflineTransactionSigner.SignTransaction(
                    account.PrivateKey,
                    to,
                    amount,
                    nonce,
                    gasPrice,
                    gasLimit
                );
                var transactionPolling = web3.TransactionManager.TransactionReceiptService;
                var transactionReceipt = await transactionPolling.SendRequestAndWaitForReceiptAsync(() =>
                    web3.Eth.Transactions.SendRawTransaction.SendRequestAsync($"0x{signedTransaction}"));

                // Return the transaction receipt in JSON
                return new OkObjectResult(JsonConvert.SerializeObject(transactionReceipt));
            }
            catch (Exception exp)
            {
                log.LogError(exp.ToString());
                return new BadRequestObjectResult(exp);
            }
        }
    }
}