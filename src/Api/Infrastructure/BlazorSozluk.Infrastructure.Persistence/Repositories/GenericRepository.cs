using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Api.Domain.Models;
using BlazorSozluk.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {

        private readonly BlazorSozlukContext _dbContext;

        protected DbSet<TEntity> entity => _dbContext.Set<TEntity>();

        public GenericRepository(BlazorSozlukContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        #region Insert Methods
        public int Add(TEntity entity)
        {
            this.entity.Add(entity);
            return _dbContext.SaveChanges();
        }

        public virtual int Add(IEnumerable<TEntity> entities)
        {
            if (entities != null && !entities.Any())
            {
                return 0;
            }
            entity.AddRange(entities);
            return _dbContext.SaveChanges();
        }

        public virtual async Task<int> AddAsync(TEntity entity)
        {
            await this.entity.AddAsync(entity);
            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> AddAsync(IEnumerable<TEntity> entities)
        {
            if (entities != null && !entities.Any())
            {
                return 0;
            }
            await entity.AddRangeAsync(entity);
            return await _dbContext.SaveChangesAsync();
        }
        #endregion

        #region UpdateMethods
        public int Update(TEntity entity)
        {
            this.entity.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            return _dbContext.SaveChanges();
        }

        public async Task<int> UpdateAsync(TEntity entity)
        {
            this.entity.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;

            return await _dbContext.SaveChangesAsync();
        }
        #endregion


        #region AddOrUpdate Methods
        public virtual int AddOrUpdate(TEntity entity)
        {
            //veritabanına gitmek yerine local'de varmı diye bakıyorum bu veri  belkide az önce cekildi bu kayıt
            if (!this.entity.Local.Any(i => EqualityComparer<Guid>.Default.Equals(i.Id, entity.Id)))
                _dbContext.Update(entity);

            return _dbContext.SaveChanges();
        }

        public Task<int> AddOrUpdateAsync(TEntity entity)
        {
            if (!this.entity.Local.Any(i => EqualityComparer<Guid>.Default.Equals(i.Id, entity.Id)))
                _dbContext.Update(entity);
            return _dbContext.SaveChangesAsync();
        }
        #endregion


        #region DeleteMethods
        public virtual int Delete(TEntity entity)
        {
            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                this.entity.Attach(entity);
            }
            this.entity.Remove(entity);

            return _dbContext.SaveChanges();
        }

        public virtual int Delete(Guid id)
        {
            var entity = this.entity.Find(id);
            return Delete(entity);
        }

        public virtual Task<int> DeleteAsync(TEntity entity)
        {
            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                this.entity.Attach(entity);
            }

            this.entity.Remove(entity);
            return _dbContext.SaveChangesAsync();
        }

        public virtual Task<int> DeleteAsync(Guid id)
        {
            var entitiy = this.entity.Find(id);
            return DeleteAsync(entitiy);
        }

        public virtual bool DeleteRange(Expression<Func<TEntity, bool>> predicate)
        {
            _dbContext.RemoveRange(entity.Where(predicate));
            return _dbContext.SaveChanges() > 0;
        }

        public virtual async Task<bool> DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate)
        {
            _dbContext.RemoveRange(predicate);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        #endregion

        #region Get Metods
        public virtual IQueryable<TEntity> AsQueryable() => entity.AsQueryable();

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = entity.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }


            query = ApplyIncludes(query, includes);

            if (noTracking)
                query = query.AsNoTracking();

            return query;
        }


        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            throw new NotImplementedException();
        }
        public Task<List<TEntity>> GetAll(bool noTracking = true)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> GetByIdAsync(Guid id, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            TEntity found = await entity.FindAsync(id);

            if (found == null)
                return null;

            if (noTracking)
                _dbContext.Entry(found).State = EntityState.Detached;

            foreach (Expression<Func<TEntity, object>> include in includes)
            {
                _dbContext.Entry(found).Reference(include).Load();
            }
            return found;
        }

        public async Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy =null , params Expression<Func<TEntity, object>>[] includes)
        {
            // entity = dbcontext sınıfında bulunun Dbset<User>Users gibi düşünebilirsin
            IQueryable<TEntity> query = entity;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            foreach (Expression<Func<TEntity, object>> include in includes)
            {
                query = query.Include(include);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (noTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.ToListAsync();
        }

        public virtual async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = entity;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            query = ApplyIncludes(query, includes);

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.SingleOrDefaultAsync();
        }

        #endregion


        #region Bulk Methods (Toplu işlemler metotları )
        public Task BulkAdd(IEnumerable<TEntity> entities)
        {
            if (entities != null && !entities.Any())
                return Task.CompletedTask;

            foreach(var entityItem in entities)
            {
                entity.Add(entityItem);
            }
            return _dbContext.SaveChangesAsync();
        }

        public Task BulkDeleteBy(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task BulkDeleteById(IEnumerable<Guid> ids)
        {
            if(ids != null && !ids.Any())
                return Task.CompletedTask;

            _dbContext.RemoveRange(entity.Where(i => ids.Contains(i.Id)));
            return _dbContext.SaveChangesAsync();
        }

        public Task BulkDeleteById(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task BulkUpdate(IEnumerable<TEntity> entities)
        {
            if (entities != null && !entities.Any())
                return Task.CompletedTask;

            foreach (var entityItem in entities)
            {
                entity.Update(entityItem);
            }
            return _dbContext.SaveChangesAsync();
        }

        #endregion

        #region SaveChanges Methods
        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
        #endregion




        private static IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query, params Expression<Func<TEntity, object>>[] includes)
        {
            if(includes!= null)
            {
                foreach(var includeItem in includes)
                {
                    query=query.Include(includeItem);
                }
                
            }

            return query;
        }




    }
}
