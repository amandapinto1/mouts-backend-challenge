namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleEventDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = string.Empty;
    public Guid SaleId { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public decimal? TotalAmount { get; set; }
    public Guid? ItemId { get; set; }
    public string? ProductName { get; set; }
    public int? Quantity { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; } = [];
}
