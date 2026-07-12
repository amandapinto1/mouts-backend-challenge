using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

public class GetCartCommand : IRequest<GetCartResult>
{
    public Guid Id { get; set; }
}
