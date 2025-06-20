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
    public class SyncEventsController : ControllerBase
    {
        private readonly ISyncEventService _syncEventService;

        public SyncEventsController(ISyncEventService syncEventService)
        {
            _syncEventService = syncEventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stopwatch = Stopwatch.StartNew();

            var events = await _syncEventService.GetAllAsync();

            stopwatch.Stop();

            var response = new ApiResponse<IEnumerable<SyncEventResponseDto>>
            {
                Success = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Sync events retrieved successfully",
                Data = events,
                DurationInMilliseconds = stopwatch.ElapsedMilliseconds
            };

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
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
