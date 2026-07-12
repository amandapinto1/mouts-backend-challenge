using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Product : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string Image { get; set; } = string.Empty;
    public decimal RatingRate { get; set; }
    public int RatingCount { get; set; }
    public Category Category { get; set; } = null!;
}
