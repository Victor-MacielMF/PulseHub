using Microsoft.AspNetCore.Mvc;
using PulseHub.Application.DTOs;
using PulseHub.Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PulseHub.API.Controllers
{
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
        /// Listar todas as mensagens da fila
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<QueueMessageResponseDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var messages = await _queueMessageService.GetAllAsync();

            return Ok(new ApiResponse<IEnumerable<QueueMessageResponseDto>>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Queue messages retrieved successfully",
                Data = messages
            });
        }

        /// <summary>
        /// Obter uma mensagem específica por ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<QueueMessageResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var message = await _queueMessageService.GetByIdAsync(id);

            if (message is null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Queue message not found",
                    Data = null
                });
            }

            return Ok(new ApiResponse<QueueMessageResponseDto>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Queue message retrieved successfully",
                Data = message
            });
        }
    }
}
