using Amplifier.AspNetCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Amplifier.AspNetCore.Repositories
{
    /// <summary>
    /// This Interface must be implemented by all repositories.
    /// </summary>
    /// <typeparam name="TEntity">Entity type of this repository</typeparam>
    /// <typeparam name="TKey">Primary key of the Entity</typeparam>
    public interface IRepositoryBase<TEntity, TKey>
           where TEntity : class, IEntity<TKey>
    {
        /// <summary>
        /// Update an entity.
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>Updated entity</returns>
        Task<TEntity> Update(TEntity entity);

        /// <summary>
        /// Get all entities by given condition.
        /// </summary>
        /// <param name="expression">Lambda Expression</param>
        /// <returns>IEnumerable of entities</returns>
        Task<IEnumerable<TEntity>> GetAllBy(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// Get an IQueryable from the entire table.
        /// </summary>
        /// <returns>IQueryable das entidades</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Create an Entity.
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <returns>Primary key type of the entity</returns>
        Task<TKey> Create(TEntity entity);

        /// <summary>
        /// Delete an entity by given Id.
        /// </summary>
        /// <param name="id">Id of the entity</param>        
        Task Delete(TKey id);

        /// <summary>
        /// Get an Entity by Id.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <returns>Entity</returns>
        Task<TEntity> GetById(TKey id);
    }
}
