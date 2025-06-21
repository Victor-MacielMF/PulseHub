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
    /// Controller responsável por gerenciar produtos.
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
        /// Retorna todos os produtos, com opção de filtrar por ativos ou inativos.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Listar todos os produtos",
            Description = "Retorna uma lista de produtos. Se o parâmetro 'isActive' não for informado, serão retornados todos os produtos (ativos e inativos). Para filtrar, utilize 'isActive=true' para produtos ativos ou 'isActive=false' para produtos inativos."
        )]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductResponseDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
        {
            var stopwatch = Stopwatch.StartNew();

            var products = await _productService.GetAllAsync(isActive);

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

        /// <summary>
        /// Retorna um produto específico pelo seu ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Obter produto por ID",
            Description = "Retorna os dados de um produto específico pelo seu ID."
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


        /// <summary>
        /// Cria um novo produto.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Criar um novo produto",
            Description = "Cadastra um novo produto no sistema."
        )]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] ProductRequestDto request)
        {
            var stopwatch = Stopwatch.StartNew();

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

        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        [HttpPut("{id:guid}")]
        [SwaggerOperation(
            Summary = "Atualizar um produto",
            Description = "Atualiza os dados de um produto específico."
        )]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductRequestDto request)
        {
            var stopwatch = Stopwatch.StartNew();

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

        /// <summary>
        /// Desativa um produto pelo seu ID (Soft Delete).
        /// </summary>
        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Desativar um produto",
            Description = "Realiza o soft delete de um produto, marcando ele como inativo no sistema."
        )]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.OK)]
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
