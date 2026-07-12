using Ambev.DeveloperEvaluation.Application.Products.ListProducts;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProductsByCategory;

public class GetProductsByCategoryResult
{
    public List<ListProductsItemResult> Data { get; set; } = [];
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
