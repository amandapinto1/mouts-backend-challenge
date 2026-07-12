using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISaleEventRepository
{
    Task PublishEventAsync(SaleEventDocument eventDocument, CancellationToken cancellationToken = default);
    Task<IEnumerable<SaleEventDocument>> GetEventsBySaleIdAsync(Guid saleId, CancellationToken cancellationToken = default);
}
