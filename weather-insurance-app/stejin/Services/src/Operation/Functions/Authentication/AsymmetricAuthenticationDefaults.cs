using System;
namespace WeatherInsurance.Operation.Functions.Authentication
{
    /// <summary>
    /// Constants for asymmetric authentication.
    /// </summary>
    public class AsymmetricAuthenticationDefaults
    {
        /// <summary>
        /// The authentication scheme
        /// </summary>
        public const string AuthenticationScheme = "AsymmetricAuthentication";

        /// <summary>
        /// The display name
        /// </summary>
        public const string DisplayName = "Asymmetric Authentication";

        internal const string EffectiveSchemeKey = "tokenvalidation:effective:";
    }
}
