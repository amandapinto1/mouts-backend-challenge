using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Categories.UpdateCategory;

public class UpdateCategoryCommand : IRequest<UpdateCategoryResult>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
