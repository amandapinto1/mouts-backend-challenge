using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleEventRepository : ISaleEventRepository
{
    private readonly IMongoCollection<SaleEventDocument> _collection;

    public SaleEventRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<SaleEventDocument>("SaleEvents");
    }

    public async Task PublishEventAsync(SaleEventDocument eventDocument, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(eventDocument, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<SaleEventDocument>> GetEventsBySaleIdAsync(Guid saleId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<SaleEventDocument>.Filter.Eq(e => e.SaleId, saleId);
        var results = await _collection.Find(filter).SortByDescending(e => e.Timestamp).ToListAsync(cancellationToken);
        return results;
    }
}
