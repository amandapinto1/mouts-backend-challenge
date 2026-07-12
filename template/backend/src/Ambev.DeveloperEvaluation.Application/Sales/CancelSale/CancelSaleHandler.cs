using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleEventRepository _eventRepository;
    private readonly ILogger<CancelSaleHandler> _logger;

    public CancelSaleHandler(ISaleRepository saleRepository, ISaleEventRepository eventRepository, ILogger<CancelSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<CancelSaleResult> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with Id {command.Id} not found");

        sale.IsCancelled = true;
        foreach (var item in sale.Items)
        {
            item.IsCancelled = true;
        }

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        var eventDoc = new SaleEventDocument
        {
            EventType = nameof(SaleCancelledEvent),
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber
        };
        await _eventRepository.PublishEventAsync(eventDoc, cancellationToken);

        _logger.LogInformation("Event Published: {EventName} - Sale {SaleNumber} cancelled",
            nameof(SaleCancelledEvent), sale.SaleNumber);

        return new CancelSaleResult { Message = $"Sale {sale.SaleNumber} cancelled successfully" };
    }
}
