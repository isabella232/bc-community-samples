using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace WeatherInsurance.Integration.Blockchain
{
    public interface IBlockchainClientRepository
    {
        IEthereumClient GetClient(string networkName);
    }

    public class BlockchainClientRepository : IBlockchainClientRepository
    {
        private readonly IDictionary<string, IEthereumClient> _clients;

        public BlockchainClientRepository(IConfiguration config)
        {
            var ethMainnet = new Web3Client(config["EthereumMainnetNode"]);
            var ethRopsten = new Web3Client(config["EthereumRopstenNode"], config["EthereumRopstenOwnerAddress"]);

            // No unlocked accounts
            IDictionary<string, IEthereumClient> clients = new Dictionary<string, IEthereumClient>();
            clients.Add("ETH:Mainnet", ethMainnet);
            clients.Add("ETH:Ropsten", ethRopsten);

            // Add local test network in dev
            // $Env:AzureWebJobsEnv = "Development"
            // export AzureWebJobsEnv=Development
            if (config["AzureWebJobsEnv"] == "Development")
            {
                var ethLocal = new Web3Client(config["EthereumLocalNode"]);
                clients.Add("ETH:Unknown", ethLocal);
            }

            // Unlocked accounts
            /*
            var ropstenOwnerAddress = config["EthereumRopstenOwnerAddress"];
            var ropstenKey = config["EthereumRopstenPrivateKey"];
            var client = new Web3Client(config["EthereumRopstenNode"], ropstenKey);
            client.SetDefaultGas(50000);
            clients.Add($"ETH:Ropsten|{ropstenOwnerAddress}", client);
            */

            _clients = clients;
        }

        public IEthereumClient GetClient(string networkName)
        {
            if (_clients.ContainsKey(networkName))
                return _clients[networkName];
            return null;
        }

    }
}
