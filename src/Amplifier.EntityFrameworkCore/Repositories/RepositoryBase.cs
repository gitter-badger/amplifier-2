using Amplifier.AspNetCore.Entities;
using Amplifier.AspNetCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Amplifier.EntityFrameworkCore.Repositories
{
    /// <summary>
    /// Base class that implements <see cref="IRepositorioBase{TEntity, TKey}"/>
    /// </summary>
    /// <typeparam name="TEntity">Entity type of this repository</typeparam>
    /// <typeparam name="TKey">Primary key of the Entity</typeparam>
    public class RepositoryBase<TEntity, TKey> : IRepositorioBase<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        /// <summary>
        /// Entity Framework DbContext.
        /// </summary>
        protected readonly DbContext _dbContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dbContext"></param>
        public RepositoryBase(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Create an Entity.
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <returns>Primary key type of the entity</returns>
        public async Task<TKey> Create(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        /// <summary>
        /// Delete an entity by given Id.
        /// </summary>
        /// <param name="id">Id of the entity</param> 
        public virtual async Task Delete(TKey id)
        {
            var entidade = (await GetAllBy(x => x is TEntity && EqualityComparer<TKey>.Default.Equals(id, (x as TEntity).Id))).FirstOrDefault();
            _dbContext.Set<TEntity>().Remove(entidade);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Get an IQueryable from the entire table.
        /// </summary>
        /// <returns>IQueryable das entidades</returns>
        public virtual IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>().AsNoTracking();
        }

        /// <summary>
        /// Get all entities by given condition.
        /// </summary>
        /// <param name="expression">Lambda Expression</param>
        /// <returns>IEnumerable of entities</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllBy(Expression<Func<TEntity, bool>> expression)
        {
            return await _dbContext.Set<TEntity>().Where(expression).ToListAsync();
        }

        /// <summary>
        /// Get an Entity by Id.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <returns>Entity</returns>
        public virtual async Task<TEntity> GetById(TKey id)
        {
            return (await GetAllBy(x => x is TEntity && EqualityComparer<TKey>.Default.Equals(id, (x as TEntity).Id))).FirstOrDefault();
        }

        /// <summary>
        /// Update an entity.
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>Updated entity</returns>
        public virtual async Task<TEntity> Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
