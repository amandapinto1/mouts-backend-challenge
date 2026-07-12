using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct;

public class GetProductHandler : IRequestHandler<GetProductCommand, GetProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<GetProductResult> Handle(GetProductCommand command, CancellationToken cancellationToken)
    {
        var validator = new GetProductCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with Id {command.Id} not found");

        return _mapper.Map<GetProductResult>(product);
    }
}
