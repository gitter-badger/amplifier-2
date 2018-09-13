namespace Amplifier.AspNetCore.Entities
{
    /// <summary>
    /// Implement this interface to define an entity for the application.
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Unique entity identifier.
        /// </summary>
        TPrimaryKey Id { get; set; }
    }
}
