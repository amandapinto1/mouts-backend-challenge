using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelItem;

public class CancelItemHandler : IRequestHandler<CancelItemCommand, CancelItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleEventRepository _eventRepository;
    private readonly ILogger<CancelItemHandler> _logger;

    public CancelItemHandler(ISaleRepository saleRepository, ISaleEventRepository eventRepository, ILogger<CancelItemHandler> logger)
    {
        _saleRepository = saleRepository;
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<CancelItemResult> Handle(CancelItemCommand command, CancellationToken cancellationToken)
    {
        var validator = new CancelItemCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with Id {command.SaleId} not found");

        var item = sale.Items.FirstOrDefault(i => i.Id == command.ItemId);
        if (item == null)
            throw new KeyNotFoundException($"Item with Id {command.ItemId} not found in sale {sale.SaleNumber}");
        item.IsCancelled = true;
        sale.CalculateTotalAmount();

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        var eventDoc = new SaleEventDocument
        {
            EventType = nameof(ItemCancelledEvent),
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            ItemId = item.Id,
            ProductId = item.ProductId,
            Quantity = item.Quantity
        };
        await _eventRepository.PublishEventAsync(eventDoc, cancellationToken);

        _logger.LogInformation("Event Published: {EventName} - Item {ProductId} (Qty: {Quantity}) cancelled in Sale {SaleNumber}",
            nameof(ItemCancelledEvent), item.ProductId, item.Quantity, sale.SaleNumber);

        return new CancelItemResult { Message = $"Item {item.ProductId} cancelled successfully" };
    }
}
