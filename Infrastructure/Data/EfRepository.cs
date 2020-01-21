using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{

    public class EfRepository<T> : IAsyncRepository<T> where T : BaseEntity
    {
        protected readonly BaseContext _dbContext;

        public EfRepository(BaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> ListAllAsync()
        {
            _dbContext.Database.BeginTransaction();
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<List<T>> ListAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public void Delete(List<T> entitys)
        {
           this._dbContext.Set<T>().RemoveRange(entitys);
           this._dbContext.SaveChanges();
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }
        
        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public T Add(T entity)
        {
             _dbContext.Set<T>().Add(entity);
             _dbContext.SaveChanges();
            return entity;
        }

        public List<T> Add(List<T> entitys)
        {
             _dbContext.Set<T>().AddRange(entitys);
             _dbContext.SaveChanges();
            return entitys;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
        }

        public async Task<List<T>> AddAsync(List<T> entitys)
        {
            await _dbContext.Set<T>().AddRangeAsync(entitys);
            await _dbContext.SaveChangesAsync();
            return entitys;
        }

        public async Task UpdateAsync(List<T> entitys)
        {
           _dbContext.Set<T>().UpdateRange(entitys);
           await _dbContext.SaveChangesAsync();
        }

        public void Update(List<T> entitys)
        {
            _dbContext.Set<T>().UpdateRange(entitys); 
            _dbContext.SaveChanges();
        }

        public async Task DeleteAsync(List<T> entitys)
        {
            _dbContext.Set<T>().RemoveRange(entitys);
            await _dbContext.SaveChangesAsync();
        }
    }
}