using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public UpdateProductHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateProductCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with Id {command.Id} not found");

        product.Title = command.Title;
        product.Price = command.Price;
        product.Description = command.Description;
        product.CategoryId = command.CategoryId;
        product.Image = command.Image;
        product.RatingRate = command.RatingRate;
        product.RatingCount = command.RatingCount;

        var updated = await _productRepository.UpdateAsync(product, cancellationToken);
        return _mapper.Map<UpdateProductResult>(updated);
    }
}
