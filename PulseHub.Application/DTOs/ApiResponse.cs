using System;
using System.Collections.Generic;

namespace PulseHub.Application.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int StatusCode { get; set; }
        public long DurationInMilliseconds { get; set; }

        public static ApiResponse<T> SuccessResponse( T data, string message = "Success", int statusCode = 200, long durationInMilliseconds = 0)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = statusCode,
                DurationInMilliseconds = durationInMilliseconds
            };
        }

        public static ApiResponse<T> SuccessResponse( string message = "Success", int statusCode = 200, long durationInMilliseconds = 0)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                StatusCode = statusCode,
                DurationInMilliseconds = durationInMilliseconds
            };
        }

        public static ApiResponse<T> ErrorResponse(List<string> errors, string message = "An error occurred", int statusCode = 500, long durationInMilliseconds = 0)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode,
                DurationInMilliseconds = durationInMilliseconds
            };
        }

        public static ApiResponse<T> ErrorResponse(string error, string message = "An error occurred", int statusCode = 500, long durationInMilliseconds = 0)
        {
            return ErrorResponse(new List<string> { error }, message, statusCode, durationInMilliseconds);
        }

    }
}
