namespace Amplifier.AspNetCore.MultiTenancy
{
    /// <summary>
    /// Implement this interface for an entity which must have a non-nullable TenantId as shadow property.
    /// </summary>
    public interface IHaveTenant
    {
    }
}
