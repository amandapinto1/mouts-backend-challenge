using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISaleEventRepository _eventRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;

    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        IBranchRepository branchRepository,
        IProductRepository productRepository,
        ISaleEventRepository eventRepository,
        IMapper mapper,
        ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _branchRepository = branchRepository;
        _productRepository = productRepository;
        _eventRepository = eventRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with Id {command.Id} not found");

        var branch = await _branchRepository.GetByIdAsync(command.BranchId, cancellationToken)
            ?? throw new KeyNotFoundException($"Branch with Id {command.BranchId} not found");

        foreach (var item in command.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken)
                ?? throw new KeyNotFoundException($"Product with Id {item.ProductId} not found");
            item.UnitPrice = product.Price;
        }

        sale.SaleNumber = command.SaleNumber;
        sale.SaleDate = command.SaleDate;
        sale.CustomerId = command.CustomerId;
        sale.BranchId = command.BranchId;
        sale.Items = _mapper.Map<List<SaleItem>>(command.Items);

        foreach (var item in sale.Items)
        {
            item.CalculateDiscount();
        }

        sale.CalculateTotalAmount();

        var updated = await _saleRepository.UpdateAsync(sale, cancellationToken);

        var eventDoc = new SaleEventDocument
        {
            EventType = nameof(SaleModifiedEvent),
            SaleId = updated.Id,
            SaleNumber = updated.SaleNumber,
            TotalAmount = updated.TotalAmount
        };
        await _eventRepository.PublishEventAsync(eventDoc, cancellationToken);

        _logger.LogInformation("Event Published: {EventName} - Sale {SaleNumber} modified with total {TotalAmount:C}",
            nameof(SaleModifiedEvent), updated.SaleNumber, updated.TotalAmount);

        return _mapper.Map<UpdateSaleResult>(updated);
    }
}
