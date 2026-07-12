using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Branches.CreateBranch;
using Ambev.DeveloperEvaluation.Application.Branches.GetBranch;
using Ambev.DeveloperEvaluation.Application.Branches.UpdateBranch;
using Ambev.DeveloperEvaluation.Application.Branches.DeleteBranch;
using Ambev.DeveloperEvaluation.Application.Branches.ListBranches;
using Microsoft.AspNetCore.Authorization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches;


[ApiController]
[Route("api/[controller]")]
public class BranchesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<BranchesController> _logger;

    public BranchesController(IMediator mediator, ILogger<BranchesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<ListBranchesResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBranches(
        [FromQuery(Name = "_page")] int page = 1,
        [FromQuery(Name = "_size")] int size = 10,
        [FromQuery(Name = "_order")] string? order = null,
        [FromQuery(Name = "name")] string? name = null,
        [FromQuery(Name = "city")] string? city = null,
        [FromQuery(Name = "state")] string? state = null,
        [FromQuery(Name = "isActive")] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new ListBranchesCommand
            {
                Page = page,
                Size = size,
                Order = order,
                Name = name,
                City = city,
                State = state,
                IsActive = isActive
            };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<ListBranchesResult>
            {
                Success = true,
                Message = "Success",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving branches");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while retrieving branches"
            });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetBranchResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranch(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetBranchCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<GetBranchResult>
            {
                Success = true,
                Message = "Success",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Branch with Id {BranchId} not found", id);
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving branch with Id {BranchId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while retrieving the branch"
            });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateBranchResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, new ApiResponseWithData<CreateBranchResult>
            {
                Success = true,
                Message = "Success",
                Data = result
            });
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for branch creation");
            return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating branch");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while creating the branch"
            });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateBranchResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBranch(Guid id, [FromBody] UpdateBranchCommand command, CancellationToken cancellationToken)
    {
        try
        {
            command.Id = id;
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<UpdateBranchResult>
            {
                Success = true,
                Message = "Success",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Branch with Id {BranchId} not found", id);
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for branch update");
            return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating branch with Id {BranchId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while updating the branch"
            });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<DeleteBranchResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBranch(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeleteBranchCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<DeleteBranchResult>
            {
                Success = true,
                Message = "Success",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Branch with Id {BranchId} not found", id);
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting branch with Id {BranchId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while deleting the branch"
            });
        }
    }
}

