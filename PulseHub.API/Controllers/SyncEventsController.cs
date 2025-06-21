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
    /// Controller responsible for managing sync events.
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
        /// Get all sync events.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(
            Summary = "List sync events",
            Description = "Returns all sync events. Status reflects the overall event process: Pending, Processing, Completed, Failed."
        )]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SyncEventResponseDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var stopwatch = Stopwatch.StartNew();

            var events = await _syncEventService.GetAllAsync();

            stopwatch.Stop();

            return Ok(new ApiResponse<IEnumerable<SyncEventResponseDto>>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Sync events retrieved successfully",
                Data = events,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Get a sync event by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Get sync event by ID",
            Description = "Returns the sync event by its ID. Error handling and retries are managed per channel in QueueMessages."
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
                    DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                    Timestamp = DateTime.UtcNow
                });
            }

            return Ok(new ApiResponse<SyncEventResponseDto>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Sync event retrieved successfully",
                Data = syncEvent,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
