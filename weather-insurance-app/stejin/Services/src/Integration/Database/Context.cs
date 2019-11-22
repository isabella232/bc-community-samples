using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WeatherInsurance.Integration.Database.Model;

namespace WeatherInsurance.Integration.Database
{
    public partial class Context : DbContext
    {
        public DbSet<Network> Networks { get; set; }
        public DbSet<ContractFile> ContractFiles { get; set; }
        public DbSet<DeployedContract> DeployedContracts { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<Purchase> Purchases { get; set; }

        public Context()
        { }

        public Context(DbContextOptions<Context> options)
        : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                var builder = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .AddUserSecrets<Context>();

                var config = builder.Build();

                optionsBuilder.UseSqlServer(config.GetConnectionString("Database"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Network>()
                .HasIndex(n => n.NetworkName)
                .IsUnique();

            modelBuilder.Entity<ContractFile>()
                .HasIndex(c => new { c.ContractFileName, c.OwnerAddress })
                .IsUnique();

            modelBuilder.Entity<DeployedContract>()
                        .HasKey(d => new { d.ContractAddress, d.NetworkId });

            modelBuilder.Entity<DeployedContract>()
                        .HasIndex(d => d.OwnerAddress);

            modelBuilder.Entity<DeployedContract>()
                .HasIndex(d => new { d.ContractName, d.NetworkId })
                .IsUnique();

            // Seed Data

            modelBuilder.Entity<Network>().HasData(new Network { NetworkId = 1, NetworkName = "ETH:Unknown", Platform = 0, ReferenceContractAddress = "0x0", Url = "" });
            modelBuilder.Entity<Network>().HasData(new Network { NetworkId = 2, NetworkName = "ETH:Mainnet", Platform = 0, ReferenceContractAddress = "0x13Cb835C47782dad075Ce7fAA1F8439b548B712D", Url = "https://etherscan.io" });
            modelBuilder.Entity<Network>().HasData(new Network { NetworkId = 3, NetworkName = "ETH:Kovan", Platform = 0, ReferenceContractAddress = "0x3422a48ebf29809bda10e264207ed94a5a819368", Url = "https://kovan.etherscan.io" });
            modelBuilder.Entity<Network>().HasData(new Network { NetworkId = 4, NetworkName = "ETH:Sokol", Platform = 0, ReferenceContractAddress = "0x64F84Fadae3F535BC02b17eD12a7Db33FBBEF29E", Url = "" });
            modelBuilder.Entity<Network>().HasData(new Network { NetworkId = 5, NetworkName = "ETH:Ropsten", Platform = 0, ReferenceContractAddress = "0x1F807D49324d83C3c5836Ad162839ba360EC834b", Url = "https://ropsten.etherscan.io" });
        }
    }
}
