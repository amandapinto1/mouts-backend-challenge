using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Categories.DeleteCategory;

public class DeleteCategoryCommand : IRequest<DeleteCategoryResult>
{
    public Guid Id { get; set; }
}
