using Ambev.DeveloperEvaluation.Application.Sales.CancelItem;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<SalesController> _logger;

    public SalesController(IMediator mediator, ILogger<SalesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region [GET]

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<ListSalesResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSales(
        [FromQuery(Name = "_page")] int page = 1,
        [FromQuery(Name = "_size")] int size = 10,
        [FromQuery(Name = "_order")] string? order = null,
        [FromQuery(Name = "saleNumber")] string? saleNumber = null,
        [FromQuery(Name = "customerId")] Guid? customerId = null,
        [FromQuery(Name = "branchId")] Guid? branchId = null,
        [FromQuery(Name = "_minDate")] DateTime? minDate = null,
        [FromQuery(Name = "_maxDate")] DateTime? maxDate = null,
        [FromQuery(Name = "_minTotal")] decimal? minTotal = null,
        [FromQuery(Name = "_maxTotal")] decimal? maxTotal = null,
        [FromQuery(Name = "isCancelled")] bool? isCancelled = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new ListSalesCommand
            {
                Page = page,
                Size = size,
                Order = order,
                SaleNumber = saleNumber,
                CustomerId = customerId,
                BranchId = branchId,
                MinDate = minDate,
                MaxDate = maxDate,
                MinTotal = minTotal,
                MaxTotal = maxTotal,
                IsCancelled = isCancelled
            };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<ListSalesResult>
            {
                Success = true,
                Message = "Sales retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sales");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while retrieving sales"
            });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetSaleCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<GetSaleResult>
            {
                Success = true,
                Message = "Sale retrieved successfully",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Sale with Id {SaleId} not found", id);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sale with Id {SaleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while retrieving the sale"
            });
        }
    }

    #endregion [GET]

    #region [POST]

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateSaleCommand
            {
                SaleDate = DateTime.UtcNow,
                CustomerId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value),
                BranchId = request.BranchId,
                Items = [.. request.Items.Select(i => new CreateSaleItemCommand
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                })]
            };

            var result = await _mediator.Send(command, cancellationToken);

            return Created(string.Empty, new ApiResponseWithData<CreateSaleResult>
            {
                Success = true,
                Message = "Sale created successfully",
                Data = result
            });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Business rule violation creating sale");
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sale");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = ex.Message ?? "An error occurred while creating the sale"
            });
        }
    }

    #endregion [POST]

    #region [PUT]

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSale([FromRoute] Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdateSaleCommand
            {
                Id = id,
                SaleDate = DateTime.UtcNow,
                CustomerId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value),
                BranchId = request.BranchId,
                Items = [.. request.Items.Select(i => new UpdateSaleItemCommand
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                })]
            };

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<UpdateSaleResult>
            {
                Success = true,
                Message = "Sale updated successfully",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Sale with Id {SaleId} not found for update", id);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Business rule violation updating sale {SaleId}", id);
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sale with Id {SaleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = ex.Message ?? "An error occurred while updating the sale"
            });
        }
    }

    #endregion [PUT]

    #region [DELETE]

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<DeleteSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeleteSaleCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<DeleteSaleResult>
            {
                Success = true,
                Message = result.Message,
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Sale with Id {SaleId} not found for deletion", id);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sale with Id {SaleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while deleting the sale"
            });
        }
    }

    #endregion [DELETE]

    #region [PATCH - Cancel]

    [HttpPatch("{id}/cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<CancelSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CancelSaleCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<CancelSaleResult>
            {
                Success = true,
                Message = result.Message,
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Sale with Id {SaleId} not found for cancellation", id);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling sale with Id {SaleId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while cancelling the sale"
            });
        }
    }

    [HttpPatch("{id}/items/{itemId}/cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<CancelItemResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelItem([FromRoute] Guid id, [FromRoute] Guid itemId, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CancelItemCommand { SaleId = id, ItemId = itemId };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<CancelItemResult>
            {
                Success = true,
                Message = result.Message,
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Item {ItemId} not found in sale {SaleId} for cancellation", itemId, id);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling item {ItemId} in sale {SaleId}", itemId, id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while cancelling the item"
            });
        }
    }

    #endregion [PATCH - Cancel]
}
