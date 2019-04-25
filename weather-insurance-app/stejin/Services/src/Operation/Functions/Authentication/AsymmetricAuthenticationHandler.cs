using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WeatherInsurance.Operation.Functions.Authentication
{
    public class AsymmetricAuthenticationHandler
    {
        private readonly AsymmetricAuthenticationOptions _options;


        /// <summary>
        /// Tries to validate a signature on the current request
        /// </summary>
        /// <returns></returns>
        public AsymmetricAuthenticationHandler(
            AsymmetricAuthenticationOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Tries to validate a signature on the current request
        /// </summary>
        /// <returns></returns>
        public async Task<AuthenticateResult> HandleAuthenticateAsync(HttpRequest request, ILogger _logger)
        {
            _logger.LogTrace("HandleAuthenticateAsync called");

            var signatureToken = _options.SignatureTokenRetriever(request);

            if (signatureToken != null)
            {

                using (var mem = new MemoryStream())
                {
                    request.EnableRewind();
                    await request.Body.CopyToAsync(mem);
                    request.Body.Position = 0;
                    mem.Position = 0;
                    using (var reader = new StreamReader(mem))
                    {
                        string body = string.Empty;
                        // Ignore body if file upload
                        if (request.ContentType != null && !request.ContentType.StartsWith("multipart/form-data", StringComparison.InvariantCultureIgnoreCase))
                            body = reader.ReadToEnd();
                        var message = $"{signatureToken.Nonce}|{request.Method.ToUpper()}|{request.Path.Value}{request.QueryString.Value}|{body}";
                        var isSignatureValid = _options.SignatureValidator(signatureToken.Signature, signatureToken.PublicKey, message);
                        if (!isSignatureValid)
                        {
                            _logger.LogWarning($"Signature invalid. PublicKey: {signatureToken.PublicKey}, Signature: {signatureToken.Signature}, Message: {message}");
                            return AuthenticateResult.Fail("Signature invalid");
                        }
                    }
                }

                // TODO: Verify bearer token if required
                // Signature is valid - Authenticate Bearer token
                /* var bearerToken = Options.BearerTokenRetriever(request);
                if (bearerToken != null)
                {
                    // Compare subject on JWT token with signature principal and delegate to Bearer authentication if they do match
                    // in which case Bearer handler will return AuthenticateResult

                    var jwtHandler = new JwtSecurityTokenHandler();
                    var jwtReader = jwtHandler.ReadJwtToken(bearerToken);
                    if (jwtReader.Subject == signatureToken.PublicKey)
                        return await (new BearerTokenAuthenticationHandler()).AuthenticateAsync("Bearer"); // TODO: Authenticate signed token
                    else
                        return AuthenticateResult.Fail("Bearer token subject id does not match signature public key");
                } */

                // Default if no bearer token present: Create claims identity from public key and authorize access
                var id = new ClaimsIdentity(AsymmetricAuthenticationDefaults.AuthenticationScheme);
                id.AddClaim(new Claim(ClaimTypes.Name, signatureToken.PublicKey));
                id.AddClaim(new Claim(JwtClaimTypes.Subject, signatureToken.PublicKey));
                id.AddClaim(new Claim(JwtClaimTypes.Name, signatureToken.PublicKey));
                var principal = new ClaimsPrincipal(id);

                return AuthenticateResult.Success(new AuthenticationTicket(principal, AsymmetricAuthenticationDefaults.AuthenticationScheme));

            }
            else
            {
                return AuthenticateResult.Fail("Signature verification failed");
            }
        }

    }
}
