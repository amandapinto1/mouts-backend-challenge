using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }

    public Sale Sale { get; set; } = null!;

    public void CalculateDiscount()
    {
        Discount = Quantity switch
        {
            > 20 => throw new DomainException("Maximum limit: 20 items per product."),
            >= 10 => 0.20m,
            >= 4 => 0.10m,
            _ => 0m
        };

        TotalAmount = Quantity * UnitPrice * (1 - Discount);
    }
}
