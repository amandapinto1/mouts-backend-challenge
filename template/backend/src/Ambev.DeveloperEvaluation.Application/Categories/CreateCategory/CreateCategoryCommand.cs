using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Categories.CreateCategory;

public class CreateCategoryCommand : IRequest<CreateCategoryResult>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
