using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Amplifier.AspNetCore.Authentication
{
    /// <summary>
    /// Signing configurations.
    /// </summary>
    public class SigningConfigurations
    {
        /// <summary>
        /// Security Key.
        /// </summary>
        public SecurityKey Key { get; }

        /// <summary>
        /// Signing credentials.
        /// </summary>
        public SigningCredentials SigningCredentials { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SigningConfigurations()
        {
            using (var provider = new RSACryptoServiceProvider(2048))
            {
                Key = new RsaSecurityKey(provider.ExportParameters(true));
            }

            SigningCredentials = new SigningCredentials(
                Key, SecurityAlgorithms.RsaSha256Signature);
        }
    }
}
