using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherInsurance.Integration.Blockchain;
using WeatherInsurance.Operation.Functions.Authentication;

namespace WeatherInsurance.Operation.Functions
{
    public abstract class Controller
    {
        protected readonly IConfigurationRoot _config;
        protected readonly AsymmetricAuthenticationHandler _auth;

        public Controller()
        {
            var builder = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .AddUserSecrets<ContractFiles>();

            _config = builder.Build();

            /*_auth = new AsymmetricAuthenticationHandler(new OptionsMonitor<AsymmetricAuthenticationOptions>
            {
                options =>
                    {
                        options.SignatureValidator = Signer.VerifySignature();
                    }
            }); */


            /*    new AsymmetricAuthenticationOptions
            {
                SignatureValidator = Signer.VerifySignature()
            }); */
        }
    }
}
