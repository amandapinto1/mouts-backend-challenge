using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Categories.ListCategories;

public class ListCategoriesCommand : IRequest<ListCategoriesResult>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Order { get; set; }
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
}
