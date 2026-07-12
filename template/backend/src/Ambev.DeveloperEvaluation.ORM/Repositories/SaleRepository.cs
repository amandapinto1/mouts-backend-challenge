using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .AsNoTracking()
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<IQueryable<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.AsNoTracking().Include(s => s.Items).AsQueryable();
        return Task.FromResult(query);
    }

    public async Task<int> GetNextSaleNumberAsync(CancellationToken cancellationToken = default)
    {
        var lastNumber = await _context.Sales
            .AsNoTracking()
            .OrderByDescending(s => s.SaleNumber)
            .Select(s => s.SaleNumber)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastNumber != null && int.TryParse(lastNumber.Replace("SALE-", ""), out var number))
            return number + 1;

        return 1;
    }
}
