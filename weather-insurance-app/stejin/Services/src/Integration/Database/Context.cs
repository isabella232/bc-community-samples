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

        public Context()
        { }

        public Context(DbContextOptions<Context> options)
        : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Used by migrations

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
                .HasIndex(d => d.ContractName)
                .IsUnique();

        }
    }
}
