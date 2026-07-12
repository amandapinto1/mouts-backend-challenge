using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Categories.CreateCategory;
using Ambev.DeveloperEvaluation.Application.Categories.GetCategory;
using Ambev.DeveloperEvaluation.Application.Categories.UpdateCategory;
using Ambev.DeveloperEvaluation.Application.Categories.DeleteCategory;
using Ambev.DeveloperEvaluation.Application.Categories.ListCategories;
using Microsoft.AspNetCore.Authorization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Categories;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(IMediator mediator, ILogger<CategoriesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<ListCategoriesResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(
        [FromQuery(Name = "_page")] int page = 1,
        [FromQuery(Name = "_size")] int size = 10,
        [FromQuery(Name = "_order")] string? order = null,
        [FromQuery(Name = "name")] string? name = null,
        [FromQuery(Name = "isActive")] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new ListCategoriesCommand
            {
                Page = page,
                Size = size,
                Order = order,
                Name = name,
                IsActive = isActive
            };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<ListCategoriesResult>
            {
                Success = true,
                Message = "Categories retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while retrieving categories"
            });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetCategoryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategory(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetCategoryCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<GetCategoryResult>
            {
                Success = true,
                Message = "Category retrieved successfully",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Category with Id {CategoryId} not found", id);
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category with Id {CategoryId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while retrieving the category"
            });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateCategoryResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, new ApiResponseWithData<CreateCategoryResult>
            {
                Success = true,
                Message = "Category created successfully",
                Data = result
            });
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for category creation");
            return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while creating the category"
            });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateCategoryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        try
        {
            command.Id = id;
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<UpdateCategoryResult>
            {
                Success = true,
                Message = "Category updated successfully",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Category with Id {CategoryId} not found", id);
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for category update");
            return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category with Id {CategoryId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while updating the category"
            });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<DeleteCategoryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeleteCategoryCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<DeleteCategoryResult>
            {
                Success = true,
                Message = "Category deleted successfully",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Category with Id {CategoryId} not found", id);
            return NotFound(new ApiResponse { Success = false, Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category with Id {CategoryId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while deleting the category"
            });
        }
    }
}
