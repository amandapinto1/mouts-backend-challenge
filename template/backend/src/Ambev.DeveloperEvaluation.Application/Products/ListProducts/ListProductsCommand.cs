using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public class ListProductsCommand : IRequest<ListProductsResult>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Order { get; set; }

    // Filters
    public string? Title { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}
