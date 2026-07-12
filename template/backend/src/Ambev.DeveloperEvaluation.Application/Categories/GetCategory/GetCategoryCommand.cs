using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Categories.GetCategory;

public class GetCategoryCommand : IRequest<GetCategoryResult>
{
    public Guid Id { get; set; }
}
