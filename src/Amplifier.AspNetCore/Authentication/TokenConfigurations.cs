namespace Amplifier.AspNetCore.Authentication
{
    /// <summary>
    /// Token default configurations.
    /// </summary>
    public class TokenConfigurations
    {
        /// <summary>
        /// Identifies the recipients that the JWT is intended for.
        /// </summary>
        public virtual string Audience { get; set; }

        /// <summary>
        /// Identifies principal that issued the JWT.
        /// </summary>
        public virtual string Issuer { get; set; }

        /// <summary>
        /// Token duration in seconds.
        /// </summary>
        public virtual int Seconds { get; set; }
    }
}
