namespace Amplifier.AspNetCore.MultiTenancy
{
    /// <summary>
    /// Implement this interface for an entity which must have a nullable TenantId as shadow property.
    /// </summary>
    public interface IMayHaveTenant
    {
    }
}
