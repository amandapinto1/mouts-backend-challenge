using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleEventRepository _eventRepository;
    private readonly ILogger<DeleteSaleHandler> _logger;

    public DeleteSaleHandler(ISaleRepository saleRepository, ISaleEventRepository eventRepository, ILogger<DeleteSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<DeleteSaleResult> Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with Id {command.Id} not found");

        var deleted = await _saleRepository.DeleteAsync(command.Id, cancellationToken);
        if (!deleted)
            throw new KeyNotFoundException($"Sale with Id {command.Id} not found");
        var eventDoc = new SaleEventDocument
        {
            EventType = nameof(SaleCancelledEvent),
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber
        };
        await _eventRepository.PublishEventAsync(eventDoc, cancellationToken);

        _logger.LogInformation("Event Published: {EventName} - Sale {SaleNumber} cancelled",
            nameof(SaleCancelledEvent), sale.SaleNumber);

        return new DeleteSaleResult { Message = "Sale deleted successfully" };
    }
}
