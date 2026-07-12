using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DefaultContext _context;

    public ProductRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<IQueryable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsNoTracking().AsQueryable();
        return Task.FromResult(query);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Select(p => p.Category.Name)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);
    }

    public Task<IQueryable<Product>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Where(p => p.Category.Name == category)
            .AsQueryable();
        return Task.FromResult(query);
    }
}
