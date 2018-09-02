namespace Amplifier.AspNetCore.Entities
{
    /// <summary>
    /// Implement this interface to define an entity for the application.
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface IEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }
}
