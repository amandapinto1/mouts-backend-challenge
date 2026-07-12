using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DefaultContext _context;

    public CategoryRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (category == null)
            return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<IQueryable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = _context.Categories.AsNoTracking().AsQueryable();
        return Task.FromResult(query);
    }
}
