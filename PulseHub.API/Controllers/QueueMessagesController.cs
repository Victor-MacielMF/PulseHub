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
    /// Controller responsible for managing queue messages (integration per channel).
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
        /// Get all queue messages.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(
            Summary = "List queue messages",
            Description = "Returns all queue messages for integrations. Tracks RetryCount, LastAttemptAt, ErrorMessage, and IsProcessed status per channel."
        )]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<QueueMessageResponseDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var stopwatch = Stopwatch.StartNew();

            var messages = await _queueMessageService.GetAllAsync();

            stopwatch.Stop();

            return Ok(new ApiResponse<IEnumerable<QueueMessageResponseDto>>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Queue messages retrieved successfully",
                Data = messages,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Get a queue message by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Get queue message by ID",
            Description = "Returns a queue message by its ID, including details like RetryCount, LastAttemptAt, ErrorMessage, and status per channel."
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
                    DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                    Timestamp = DateTime.UtcNow
                });
            }

            return Ok(new ApiResponse<QueueMessageResponseDto>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Queue message retrieved successfully",
                Data = message,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
