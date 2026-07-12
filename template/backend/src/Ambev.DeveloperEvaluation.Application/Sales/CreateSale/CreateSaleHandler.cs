using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISaleEventRepository _eventRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleHandler(
        ISaleRepository saleRepository,
        IBranchRepository branchRepository,
        IProductRepository productRepository,
        ISaleEventRepository eventRepository,
        IMapper mapper,
        ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _branchRepository = branchRepository;
        _productRepository = productRepository;
        _eventRepository = eventRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Resolve BranchName
        var branch = await _branchRepository.GetByIdAsync(command.BranchId, cancellationToken)
            ?? throw new KeyNotFoundException($"Branch with Id {command.BranchId} not found");
        command.BranchName = branch.Name;

        // Auto-generate SaleNumber
        var nextNumber = await _saleRepository.GetNextSaleNumberAsync(cancellationToken);
        command.SaleNumber = $"SALE-{nextNumber:D6}";

        // Resolve ProductName and UnitPrice for each item
        foreach (var item in command.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken)
                ?? throw new KeyNotFoundException($"Product with Id {item.ProductId} not found");
            item.ProductName = product.Title;
            item.UnitPrice = product.Price;
        }

        var sale = _mapper.Map<Sale>(command);

        foreach (var item in sale.Items)
        {
            item.CalculateDiscount();
        }

        sale.CalculateTotalAmount();

        var created = await _saleRepository.CreateAsync(sale, cancellationToken);

        var eventDoc = new SaleEventDocument
        {
            EventType = nameof(SaleCreatedEvent),
            SaleId = created.Id,
            SaleNumber = created.SaleNumber,
            TotalAmount = created.TotalAmount
        };
        await _eventRepository.PublishEventAsync(eventDoc, cancellationToken);

        _logger.LogInformation("Event Published: {EventName} - Sale {SaleNumber} created with total {TotalAmount:C}",
            nameof(SaleCreatedEvent), created.SaleNumber, created.TotalAmount);

        return _mapper.Map<CreateSaleResult>(created);
    }
}
