using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IQueryable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
}
