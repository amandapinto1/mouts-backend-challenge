using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class CartRepository : ICartRepository
{
    private readonly DefaultContext _context;

    public CartRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Cart> CreateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        await _context.Carts.AddAsync(cart, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return cart;
    }

    public async Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .AsNoTracking()
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Cart> UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        _context.Carts.Update(cart);
        await _context.SaveChangesAsync(cancellationToken);
        return cart;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Carts
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (cart == null)
            return false;

        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<IQueryable<Cart>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = _context.Carts.AsNoTracking().Include(c => c.Products).AsQueryable();
        return Task.FromResult(query);
    }
}
