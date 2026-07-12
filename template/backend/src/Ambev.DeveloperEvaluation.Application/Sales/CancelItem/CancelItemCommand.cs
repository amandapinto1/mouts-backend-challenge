using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelItem;

public class CancelItemCommand : IRequest<CancelItemResult>
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
}
