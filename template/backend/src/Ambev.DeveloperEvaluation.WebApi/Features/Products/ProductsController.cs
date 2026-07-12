using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetCategories;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetProductsByCategory;
using Ambev.DeveloperEvaluation.Application.Products.ListProducts;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region [GET]

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<ListProductsResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
    [FromQuery(Name = "_page")] int page = 1,
    [FromQuery(Name = "_size")] int size = 10,
    [FromQuery(Name = "_order")] string? order = null,
    [FromQuery(Name = "title")] string? title = null,
    [FromQuery(Name = "categoryId")] Guid? categoryId = null,
    [FromQuery(Name = "_minPrice")] decimal? minPrice = null,
    [FromQuery(Name = "_maxPrice")] decimal? maxPrice = null,
    CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new ListProductsCommand
            {
                Page = page,
                Size = size,
                Order = order,
                Title = title,
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<ListProductsResult>
            {
                Success = true,
                Message = "Products retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while retrieving products"
            });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetProductResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetProductCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<GetProductResult>
            {
                Success = true,
                Message = "Product retrieved successfully",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product with Id {ProductId} not found", id);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with Id {ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while retrieving the product"
            });
        }
    }

    [HttpGet("categories")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetCategoriesCommand();
            var result = await _mediator.Send(command, cancellationToken);

            return base.Ok(result.Categories);
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

    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetProductsByCategoryResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductsByCategory(
        [FromRoute] string category,
        [FromQuery(Name = "_page")] int page = 1,
        [FromQuery(Name = "_size")] int size = 10,
        [FromQuery(Name = "_order")] string? order = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new GetProductsByCategoryCommand
            {
                Category = category,
                Page = page,
                Size = size,
                Order = order
            };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<GetProductsByCategoryResult>
            {
                Success = true,
                Message = "Products retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products for category {Category}", category);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while retrieving products by category"
            });
        }
    }

    #endregion [GET]

    #region [POST]

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateProductResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);

            return Created(string.Empty, new ApiResponseWithData<CreateProductResult>
            {
                Success = true,
                Message = "Product created successfully",
                Data = result
            });
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error creating product");
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while creating the product"
            });
        }
    }

    #endregion [POST]

    #region [PUT]

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateProductResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
    {
        try
        {
            command.Id = id;
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<UpdateProductResult>
            {
                Success = true,
                Message = "Product updated successfully",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product with Id {ProductId} not found for update", id);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with Id {ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while updating the product"
            });
        }
    }
    
    #endregion [PUT]

    #region [DELETE]

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<DeleteProductResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeleteProductCommand { Id = id };
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<DeleteProductResult>
            {
                Success = true,
                Message = result.Message,
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product with Id {ProductId} not found for deletion", id);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with Id {ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
            {
                Success = false,
                Message = "An error occurred while deleting the product"
            });
        }
    }

    #endregion [DELETE]


}
