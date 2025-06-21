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
    /// Controller responsável por consultar as mensagens publicadas nas filas de integração.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class QueueMessagesController : ControllerBase
    {
        private readonly IQueueMessageService _queueMessageService;

        public QueueMessagesController(IQueueMessageService queueMessageService)
        {
            _queueMessageService = queueMessageService;
        }

        /// <summary>
        /// Retorna todas as mensagens da fila.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Listar mensagens da fila",
            Description = "Retorna uma lista de todas as mensagens publicadas nas filas de integração."
        )]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<QueueMessageResponseDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var stopwatch = Stopwatch.StartNew();

            var messages = await _queueMessageService.GetAllAsync();

            stopwatch.Stop();

            var response = new ApiResponse<IEnumerable<QueueMessageResponseDto>>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Queue messages retrieved successfully",
                Data = messages,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            };

            return Ok(response);
        }

        /// <summary>
        /// Retorna uma mensagem da fila específica pelo ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Obter mensagem da fila por ID",
            Description = "Retorna os dados de uma mensagem da fila específica pelo seu ID."
        )]
        [ProducesResponseType(typeof(ApiResponse<QueueMessageResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var stopwatch = Stopwatch.StartNew();

            var message = await _queueMessageService.GetByIdAsync(id);

            stopwatch.Stop();

            if (message is null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Queue message not found",
                    Data = null,
                    DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                    Timestamp = DateTime.UtcNow
                });
            }

            var response = new ApiResponse<QueueMessageResponseDto>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Queue message retrieved successfully",
                Data = message,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            };

            return Ok(response);
        }
    }
}
