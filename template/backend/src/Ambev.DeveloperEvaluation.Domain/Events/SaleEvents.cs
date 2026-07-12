namespace Ambev.DeveloperEvaluation.Domain.Events;

public record SaleCreatedEvent(Guid SaleId, string SaleNumber, decimal TotalAmount);
public record SaleModifiedEvent(Guid SaleId, string SaleNumber, decimal TotalAmount);
public record SaleCancelledEvent(Guid SaleId, string SaleNumber);
public record ItemCancelledEvent(Guid SaleId, Guid ItemId, string ProductName, int Quantity);
