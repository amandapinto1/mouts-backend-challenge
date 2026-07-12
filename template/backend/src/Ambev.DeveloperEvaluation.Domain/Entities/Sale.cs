using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : AuditableEntity
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }

    public List<SaleItem> Items { get; set; } = [];

    public void CalculateTotalAmount()
    {
        TotalAmount = Items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount);
    }
}
