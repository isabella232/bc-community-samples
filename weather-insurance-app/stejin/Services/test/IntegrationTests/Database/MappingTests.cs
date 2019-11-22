using System;
using System.Numerics;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherInsurance.Integration.Database.Mappings;

namespace IntegrationTests.Database
{
    [TestClass]
    public class MappingTests
    {
        [TestCategory("Mock")]
        [TestMethod]
        public async Task PurchaseDataModelToDomainModelTest()
        {
            var context = DatabaseContextFactory.CreateMockContext("PurchaseDataModelToDomainModelTest");

            var network = new WeatherInsurance.Integration.Database.Model.Network() { NetworkId = 1, Platform = 1, NetworkName = "TestNetwork" };
            context.Networks.Add(network);

            var contractFile = new WeatherInsurance.Integration.Database.Model.ContractFile() { OwnerAddress = "0xo123" };
            context.ContractFiles.Add(contractFile);

            var contract = new WeatherInsurance.Integration.Database.Model.DeployedContract() { ContractAddress = "0xc123", ContractFile = contractFile, Network = network, NetworkId = network.NetworkId, ContractFileId = contractFile.ContractFileId };
            context.DeployedContracts.Add(contract);

            decimal premium = 300.123m;
            var purchase = new WeatherInsurance.Integration.Database.Model.Purchase() { PurchaseId = 1, UserAddress = "0xu123", Contract = contract, ContractAddress = contract.ContractAddress, NetworkId = network.NetworkId, Notional = 3000000, Premium = premium };
            context.Purchases.Add(purchase);

            context.SaveChanges();

            var mappings = new MapperConfigurationExpression();
            mappings.AddProfile(new PurchaseMapperProfile());
            mappings.AddProfile(new DeployedContractMapperProfile());
            mappings.AddProfile(new NetworkMapperProfile());
            mappings.AddProfile(new ContractFileMapperProfile());

            var config = new MapperConfiguration(mappings);
            var mapper = config.CreateMapper();

            var test = context.Purchases.Find(purchase.PurchaseId);

            //var result = mapper.Map<WeatherInsurance.Domain.Model.Purchase>(test);

            var repo = new WeatherInsurance.Integration.Database.PurchaseRepository(context);
            var result = await repo.Get(purchase.PurchaseId);

            Assert.AreEqual(purchase.UserAddress, result.UserAddress);
            Assert.AreEqual(premium, result.Premium);

        }
    }
}
