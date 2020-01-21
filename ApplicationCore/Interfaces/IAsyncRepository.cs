using System;
using System.Collections.Generic;
using ApplicationCore.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ApplicationCore.Interfaces
{
    public interface IAsyncRepository<T> where T:BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> ListAllAsync();
        Task<List<T>> ListAsync(ISpecification<T> spec);
        Task<T> AddAsync(T entity);
        T Add(T entity);
        Task<List<T>> AddAsync(List<T> entitys);
        List<T> Add(List<T> entities);
        Task UpdateAsync(T entity);
        void Update(T entity);
        Task DeleteAsync(T entity);
        void Delete(T entity);
        Task UpdateAsync(List<T> entitys);
        void Update(List<T> entities);
        Task DeleteAsync(List<T> entitys);
        void Delete(List<T> entitys);
        Task<int> CountAsync(ISpecification<T> spec);
    }
}
