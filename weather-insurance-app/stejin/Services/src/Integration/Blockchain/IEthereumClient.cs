using System;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace WeatherInsurance.Integration.Blockchain
{
 public interface IEthereumClient
    {
        Task<string> GetDefaultAccount();

        Task<string[]> GetAccounts();

        Task<string> GetContractCode(string contractAddress);

        Contract GetContract(string abi, string contractAddress);

        Task<string> Sign(string signerAddress, string message);

        Task<Transaction> GetTransaction(string hash);
    }
}
