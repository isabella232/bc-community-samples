using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using WeatherInsurance.Domain.Model;
using WeatherInsurance.Integration.AzureStorage.Blobs;
using WeatherInsurance.Integration.Blockchain;
using WeatherInsurance.Integration.Database;
using WeatherInsurance.Operation.Functions;
using WeatherInsurance.Operation.Functions.Authentication;

[assembly: WebJobsStartup(typeof(Startup), "Web Jobs Extension")]
namespace WeatherInsurance.Operation.Functions
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {

            builder.AddExtension<InjectConfiguration>();

        }
    }

    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class InjectAttribute : Attribute
    {
        public Type Type { get; }
        public InjectAttribute(Type type) => Type = type;
    }

    public class InjectConfiguration : IExtensionConfigProvider
    {
        private IServiceProvider _serviceProvider;

        public void Initialize(ExtensionConfigContext context)
        {
            var services = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

            var config = builder.Build();

            //services.AddDbContext<WeatherInsurance.Integration.Database.Context>(options =>
            //      options.UseSqlServer(config.GetConnectionString("Database")), ServiceLifetime.Transient);

            var auth = new AsymmetricAuthenticationHandler(new AsymmetricAuthenticationOptions
            {
                SignatureValidator = Signer.VerifySignature()
            });

            services.AddSingleton(auth);

            var storageAccount = CloudStorageAccount.Parse(config.GetWebJobsConnectionString("AzureWebJobsStorage"));
            services.AddSingleton(storageAccount);

            services.AddSingleton<IBlockchainClientRepository>(new BlockchainClientRepository(config));

            services.AddSingleton<IBlobContractFileRepository, BlobContractFileRepository>();
            services.AddSingleton<IReferenceBlobContractFileRepository, ReferenceBlobContractFileRepository>();

            services.AddTransient<Context>(_ => new Context());

            services.AddTransient<IRepository<Network>, NetworkRepository>();
            services.AddTransient<IRepository<ContractFile>, ContractFileRepository>();
            services.AddTransient<IRepository<DeployedContract>, DeployedContractRepository>();
            services.AddTransient<IRepository<Fee>, FeeRepository>();
            services.AddTransient<IRepository<Purchase>, PurchaseRepository>();

            IDictionary<string, ContractEventListener<InsuranceBoughtEvent>> blockChainEventListeners = new Dictionary<string, ContractEventListener<InsuranceBoughtEvent>>();
            services.AddSingleton(blockChainEventListeners);

            _serviceProvider = services.BuildServiceProvider(true);

            context
                .AddBindingRule<InjectAttribute>()
                .BindToInput<dynamic>(i => _serviceProvider.GetRequiredService(i.Type));
        }
    }
}