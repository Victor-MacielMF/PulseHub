using Microsoft.AspNetCore.Mvc;
using PulseHub.Application.DTOs;
using PulseHub.Application.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace PulseHub.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing products.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get all products with optional filter for active or inactive.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(
            Summary = "List all products",
            Description = "Returns all products. Use the 'isActive' query parameter to filter by active (true) or inactive (false) products. If not provided, returns all products regardless of status."
        )]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductResponseDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
        {
            var stopwatch = Stopwatch.StartNew();

            var products = await _productService.GetAllAsync(isActive);

            stopwatch.Stop();

            return Ok(new ApiResponse<IEnumerable<ProductResponseDto>>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Products retrieved successfully",
                Data = products,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Get a product by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Get product by ID",
            Description = "Returns product details for the specified ID."
        )]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var stopwatch = Stopwatch.StartNew();

            var product = await _productService.GetByIdAsync(id);

            stopwatch.Stop();

            if (product is null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Product not found",
                    DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                    Timestamp = DateTime.UtcNow
                });
            }

            return Ok(new ApiResponse<ProductResponseDto>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Product retrieved successfully",
                Data = product,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Create a new product.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new product",
            Description = "Registers a new product in the system."
        )]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] ProductRequestDto request)
        {
            var stopwatch = Stopwatch.StartNew();

            var createdProduct = await _productService.CreateAsync(request);

            stopwatch.Stop();

            return CreatedAtAction(nameof(GetById), new { id = createdProduct.ProductId }, new ApiResponse<ProductResponseDto>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.Created,
                Message = "Product created successfully",
                Data = createdProduct,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        [HttpPut("{id:guid}")]
        [SwaggerOperation(
            Summary = "Update a product",
            Description = "Updates the details of an existing product."
        )]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductRequestDto request)
        {
            var stopwatch = Stopwatch.StartNew();

            var updatedProduct = await _productService.UpdateAsync(id, request);

            stopwatch.Stop();

            return Ok(new ApiResponse<ProductResponseDto>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Product updated successfully",
                Data = updatedProduct,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Soft delete a product by marking it as inactive.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Deactivate a product",
            Description = "Performs a soft delete by marking the product as inactive instead of removing it from the database."
        )]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var stopwatch = Stopwatch.StartNew();

            await _productService.DeleteAsync(id);

            stopwatch.Stop();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.NoContent,
                Message = "Product deactivated successfully",
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
