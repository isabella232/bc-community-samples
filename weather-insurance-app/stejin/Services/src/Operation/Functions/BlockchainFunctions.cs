using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Nethereum.Util;
using WeatherInsurance.Domain.Model;
using WeatherInsurance.Integration.AzureStorage.Blobs;
using WeatherInsurance.Integration.Blockchain;
using WeatherInsurance.Integration.Database;

namespace WeatherInsurance.Operation.Functions
{
    public static class BlockchainFunctions
    {
        [FunctionName("feepaymentsmonitor")]
        public static async Task MonitorFeePayments([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log,
            [Inject(typeof(IBlockchainClientRepository))] IBlockchainClientRepository blockchainRepo,
            [Inject(typeof(IRepository<Fee>))] IRepository<Fee> feeRepo,
            [Inject(typeof(IRepository<DeployedContract>))] IRepository<DeployedContract> contractRepo)
        {
            log.LogInformation($"Monitor Fee Payments trigger function executed at: {DateTime.Now}");

            // Uncomfirmed fees not older than 1 hour
            var cutOff = DateTime.UtcNow.AddHours(-1);
            var uncomfirmedFees = await feeRepo.Find(f => !f.IsConfirmed && f.Timestamp >= cutOff);
            foreach (var fee in uncomfirmedFees)
            {
                var client = blockchainRepo.GetClient(fee.Contract.Network.Name);
                if (client != null)
                {
                    var transaction = await client.GetTransaction(fee.TransactionHash);
                    var paidAmount = transaction.Value ?? transaction.Value.Value;
                    if (paidAmount != 0 && paidAmount >= fee.Contract.GetRequiredFeeAmount())
                    {
                        // Complete fee record and register contract
                        fee.IsConfirmed = true;
                        await feeRepo.Update(fee);

                        var contract = await contractRepo.Get(fee.Contract.Address);
                        contract.IsRegistered = true;
                        await contractRepo.Update(contract);

                        log.LogInformation($"{contract.Address}: {paidAmount} received.");
                    }
                }
            }
        }

        [FunctionName("insuranceboughteventslistener")]
        public static async Task ListenForInsuranceBoughtEvents([TimerTrigger("*/30 * * * * *")]TimerInfo myTimer, ILogger log,
            [Inject(typeof(IBlockchainClientRepository))] IBlockchainClientRepository blockchainRepo,
            [Inject(typeof(IBlobContractFileRepository))] IBlobContractFileRepository blobContractFileRepository,
            [Inject(typeof(IDictionary<string, ContractEventListener<InsuranceBoughtEvent>>))] IDictionary<string, ContractEventListener<InsuranceBoughtEvent>> listeners,
            [Inject(typeof(IRepository<Purchase>))] IRepository<Purchase> purchaseRepo,
            [Inject(typeof(IRepository<DeployedContract>))] IRepository<DeployedContract> contractRepo)
        {
            try
            {
                log.LogInformation($"Listen for Insurance Bought Events trigger function executed at: {DateTime.Now}");

                var activeContracts = await contractRepo.Find(c => c.ExpirationDateTime >= DateTime.UtcNow);

                foreach (var contract in activeContracts)
                {
                    if (listeners.ContainsKey(contract.Address))
                        continue;

                    var client = blockchainRepo.GetClient(contract.Network.Name);
                    if (client == null)
                        continue;

                    var blobContractFile = await blobContractFileRepository.GetBlobContractFile(contract.OwnerAddress, contract.ContractFile.Name, true, true, false);

                    var listener = new ContractEventListener<InsuranceBoughtEvent>(client, blobContractFile.Abi.ToString(), contract.Address, "InsuranceBought",
                        (string contractAddress, InsuranceBoughtEvent capturedEvent) =>
                        {
                            var purchase = new Purchase()
                            {
                                Contract = contract,
                                UserAddress = capturedEvent.User,
                                Notional = UnitConversion.Convert.FromWei(capturedEvent.Notional),
                                Premium = UnitConversion.Convert.FromWei(capturedEvent.Premium),
                                Timestamp = DateTime.UtcNow
                            };
                            var result = purchaseRepo.AddNew(purchase).Result;

                            log.LogInformation($"Insurance bought: {contractAddress} {capturedEvent.Notional} {capturedEvent.Premium}");
                        },
                        log);
                    await listener.Start();

                    listeners.Add(contract.Address, listener);
                }

                foreach (var listener in listeners)
                {
                    await listener.Value.GetChanges();
                }

            }
            catch (Exception ex)
            {
                log.LogError($"Insurance Bought Events trigger error: {ex.Message}");
                throw ex;
            }
            finally
            {
                log.LogInformation($"Insurance Bought Events trigger finished at {DateTime.UtcNow.ToString()}");
            }

        }
    }
}
