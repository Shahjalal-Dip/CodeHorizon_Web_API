using System.Net;
using System.Text.Json;
using CodeHorizon.Core.Exceptions;
using CodeHorizon.Application.DTOs;

namespace CodeHorizon.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponseDto
            {
                Instance = context.Request.Path,
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case ValidationException validationEx:
                    response.StatusCode = validationEx.StatusCode;
                    errorResponse.Status = validationEx.StatusCode;
                    errorResponse.Title = "Validation Failed";
                    errorResponse.Detail = validationEx.Message;
                    errorResponse.ErrorCode = validationEx.ErrorCode;
                    errorResponse.Errors = validationEx.Errors;
                    break;

                case NotFoundException notFoundEx:
                    response.StatusCode = notFoundEx.StatusCode;
                    errorResponse.Status = notFoundEx.StatusCode;
                    errorResponse.Title = "Resource Not Found";
                    errorResponse.Detail = notFoundEx.Message;
                    errorResponse.ErrorCode = notFoundEx.ErrorCode;
                    break;

                case UnauthorizedException unauthorizedEx:
                    response.StatusCode = unauthorizedEx.StatusCode;
                    errorResponse.Status = unauthorizedEx.StatusCode;
                    errorResponse.Title = "Unauthorized";
                    errorResponse.Detail = unauthorizedEx.Message;
                    errorResponse.ErrorCode = unauthorizedEx.ErrorCode;
                    break;

                case ForbiddenException forbiddenEx:
                    response.StatusCode = forbiddenEx.StatusCode;
                    errorResponse.Status = forbiddenEx.StatusCode;
                    errorResponse.Title = "Forbidden";
                    errorResponse.Detail = forbiddenEx.Message;
                    errorResponse.ErrorCode = forbiddenEx.ErrorCode;
                    break;

                case ConflictException conflictEx:
                    response.StatusCode = conflictEx.StatusCode;
                    errorResponse.Status = conflictEx.StatusCode;
                    errorResponse.Title = "Conflict";
                    errorResponse.Detail = conflictEx.Message;
                    errorResponse.ErrorCode = conflictEx.ErrorCode;
                    break;

                case BadRequestException badRequestEx:
                    response.StatusCode = badRequestEx.StatusCode;
                    errorResponse.Status = badRequestEx.StatusCode;
                    errorResponse.Title = "Bad Request";
                    errorResponse.Detail = badRequestEx.Message;
                    errorResponse.ErrorCode = badRequestEx.ErrorCode;
                    break;

                default:
                    // Log unexpected errors
                    _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Status = response.StatusCode;
                    errorResponse.Title = "An error occurred while processing your request";
                    errorResponse.ErrorCode = "INTERNAL_SERVER_ERROR";

                    if (_environment.IsDevelopment())
                    {
                        errorResponse.Detail = exception.Message;
                        errorResponse.Errors = new Dictionary<string, string[]>
                        {
                            ["stackTrace"] = new[] { exception.StackTrace ?? "No stack trace available" }
                        };
                    }
                    else
                    {
                        errorResponse.Detail = "An internal error occurred. Please try again later.";
                    }
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(jsonResponse);
        }
    }
}