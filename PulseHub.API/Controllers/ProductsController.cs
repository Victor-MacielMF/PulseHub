using Microsoft.AspNetCore.Mvc;
using PulseHub.Application.DTOs;
using PulseHub.Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace PulseHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stopwatch = Stopwatch.StartNew();

            var products = await _productService.GetAllAsync();

            stopwatch.Stop();

            var response = new ApiResponse<IEnumerable<ProductResponseDto>>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Products retrieved successfully",
                Data = products,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds
            };

            return Ok(response);
        }

        // GET: api/products/{id}
        [HttpGet("{id:guid}")]
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
                    DurationInMilliseconds = stopwatch.ElapsedMilliseconds
                });
            }

            var response = new ApiResponse<ProductResponseDto>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Product retrieved successfully",
                Data = product,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds
            };

            return Ok(response);
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductRequestDto request)
        {
            var stopwatch = Stopwatch.StartNew();

            if (!ModelState.IsValid)
            {
                stopwatch.Stop();
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request data",
                    Errors = new List<string> { "Validation errors in request" },
                    DurationInMilliseconds = stopwatch.ElapsedMilliseconds
                });
            }

            var createdProduct = await _productService.CreateAsync(request);

            stopwatch.Stop();

            var response = new ApiResponse<ProductResponseDto>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.Created,
                Message = "Product created successfully",
                Data = createdProduct,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds
            };

            return CreatedAtAction(nameof(GetById), new { id = createdProduct.ProductId }, response);
        }

        // PUT: api/products/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductRequestDto request)
        {
            var stopwatch = Stopwatch.StartNew();

            if (!ModelState.IsValid)
            {
                stopwatch.Stop();
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request data",
                    Errors = new List<string> { "Validation errors in request" },
                    DurationInMilliseconds = stopwatch.ElapsedMilliseconds
                });
            }

            var updatedProduct = await _productService.UpdateAsync(id, request);

            stopwatch.Stop();

            var response = new ApiResponse<ProductResponseDto>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Product updated successfully",
                Data = updatedProduct,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds
            };

            return Ok(response);
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var stopwatch = Stopwatch.StartNew();

            await _productService.DeleteAsync(id);

            stopwatch.Stop();

            var response = new ApiResponse<object>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.NoContent,
                Message = "Product deleted successfully",
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds
            };

            return Ok(response);
        }
    }
}
