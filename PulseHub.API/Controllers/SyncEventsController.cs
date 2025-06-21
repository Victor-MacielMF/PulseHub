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
    /// Controller responsável por consultar os eventos de sincronização dos produtos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SyncEventsController : ControllerBase
    {
        private readonly ISyncEventService _syncEventService;

        public SyncEventsController(ISyncEventService syncEventService)
        {
            _syncEventService = syncEventService;
        }

        /// <summary>
        /// Retorna todos os eventos de sincronização.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Listar eventos de sincronização",
            Description = "Retorna uma lista de todos os eventos de sincronização registrados no sistema."
        )]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SyncEventResponseDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var stopwatch = Stopwatch.StartNew();

            var events = await _syncEventService.GetAllAsync();


            var response = new ApiResponse<IEnumerable<SyncEventResponseDto>>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Sync events retrieved successfully",
                Data = events,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds
            };

            stopwatch.Stop();

            return Ok(response);
        }

        /// <summary>
        /// Retorna um evento de sincronização específico pelo ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Obter evento de sincronização por ID",
            Description = "Retorna os dados de um evento de sincronização específico pelo seu ID."
        )]
        [ProducesResponseType(typeof(ApiResponse<SyncEventResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var stopwatch = Stopwatch.StartNew();

            var syncEvent = await _syncEventService.GetByIdAsync(id);

            stopwatch.Stop();

            if (syncEvent is null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Sync event not found",
                    DurationInMilliseconds = stopwatch.ElapsedMilliseconds
                });
            }

            var response = new ApiResponse<SyncEventResponseDto>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Sync event retrieved successfully",
                Data = syncEvent,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds
            };

            return Ok(response);
        }
    }
}
