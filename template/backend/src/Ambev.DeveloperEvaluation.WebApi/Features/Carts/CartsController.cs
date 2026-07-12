using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.Application.Carts.ListCarts;
using Microsoft.AspNetCore.Authorization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<CartsController> _logger;

    public CartsController(IMediator mediator, ILogger<CartsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region [GET]

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<ListCartsResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCarts(
    [FromQuery(Name = "_page")] int page = 1,
    [FromQuery(Name = "_size")] int size = 10,
    [FromQuery(Name = "_order")] string? order = null,
    [FromQuery(Name = "userId")] Guid? userId = null,
    [FromQuery(Name = "_minDate")] DateTime? minDate = null,
    [FromQuery(Name = "_maxDate")] DateTime? maxDate = null,
    CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new ListCartsCommand
            {
                Page = page,
                Size = size,
                Order = order,
                UserId = userId,
                MinDate = minDate,
                MaxDate = maxDate
            };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<ListCartsResult>
            {
                Success = true,
                Message = "Success",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving carts");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while retrieving carts"
            });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetCartResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCart([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetCartCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<GetCartResult>
            {
                Success = true,
                Message = "Success",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Cart with Id {CartId} not found", id);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cart with Id {CartId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while retrieving the cart"
            });
        }
    }

    #endregion [GET]

    #region [POST]

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateCartResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCart([FromBody] CreateCartRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateCartCommand
            {
                UserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value),
                Date = request.Date,
                Products = request.Products
            };
            var result = await _mediator.Send(command, cancellationToken);

            return Created(string.Empty, new ApiResponseWithData<CreateCartResult>
            {
                Success = true,
                Message = "Success",
                Data = result
            });
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error creating cart");
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating cart");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while creating the cart"
            });
        }
    }

    #endregion [POST]

    #region [PUT]

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateCartResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCart([FromRoute] Guid id, [FromBody] UpdateCartRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdateCartCommand
            {
                Id = id,
                UserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value),
                Date = request.Date,
                Products = request.Products
            };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<UpdateCartResult>
            {
                Success = true,
                Message = "Cart updated successfully",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Cart with Id {CartId} not found for update", id);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart with Id {CartId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while updating the cart"
            });
        }
    }

    #endregion [PUT]

    #region [DELETE]

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<DeleteCartResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCart([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeleteCartCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<DeleteCartResult>
            {
                Success = true,
                Message = result.Message,
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Cart with Id {CartId} not found for deletion", id);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting cart with Id {CartId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while deleting the cart"
            });
        }
    }

    #endregion [PUT]
}
