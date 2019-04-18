using System;
using Microsoft.AspNetCore.Http;

namespace WeatherInsurance.Operation.Functions.Authentication
{
    /// <summary>
    /// Asymmetric authentication options.
    /// </summary>
    public class AsymmetricAuthenticationOptions
    {
        /// <summary>
        /// Callback to retrieve Bearer token from incoming request
        /// </summary>
        public Func<HttpRequest, string> BearerTokenRetriever { get; set; } = TokenRetrieval.FromAuthorizationHeader();

        /// <summary>
        /// Gets or sets the signature token retriever.
        /// </summary>
        /// <value>The signature token retriever.</value>
        public Func<HttpRequest, AuthenticationToken> SignatureTokenRetriever { get; set; } = TokenRetrieval.FromAuthenticationHeader();

        /// <summary>
        /// Gets or sets the signature validator.
        /// Args: Signature, PublicKey, Message
        /// </summary>
        /// <value>The signature validator.</value>
        public Func<string, string, string, bool> SignatureValidator { get; set; }
    }
}
