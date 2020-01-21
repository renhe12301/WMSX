using System;
using System.Collections.Generic;
using ApplicationCore.Entities;
using System.Threading.Tasks;
namespace ApplicationCore.Interfaces
{
    public interface IAsyncRepository<T> where T:BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> ListAllAsync();
        Task<List<T>> ListAsync(ISpecification<T> spec);
        Task<T> AddAsync(T entity);
        Task<List<T>> AddAsync(List<T> entitys);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task UpdateAsync(List<T> entitys);
        Task DeleteAsync(List<T> entitys);
        Task<int> CountAsync(ISpecification<T> spec);
    }
}
