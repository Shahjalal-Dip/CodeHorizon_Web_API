using System;

namespace CodeHorizon.Core.Exceptions
{
    public abstract class ApiException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }

        protected ApiException(string message, int statusCode, string errorCode)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }

    public class NotFoundException : ApiException
    {
        public NotFoundException(string resourceName, string identifier)
            : base($"{resourceName} with identifier '{identifier}' was not found", 404, "RESOURCE_NOT_FOUND")
        {
        }
    }

    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message = "Authentication is required")
            : base(message, 401, "UNAUTHORIZED")
        {
        }
    }

    public class ForbiddenException : ApiException
    {
        public ForbiddenException(string message = "You don't have permission to access this resource")
            : base(message, 403, "FORBIDDEN")
        {
        }
    }

    public class ConflictException : ApiException
    {
        public ConflictException(string message)
            : base(message, 409, "CONFLICT")
        {
        }
    }

    public class ValidationException : ApiException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("Validation failed", 400, "VALIDATION_ERROR")
        {
            Errors = errors;
        }
    }

    public class BadRequestException : ApiException
    {
        public BadRequestException(string message)
            : base(message, 400, "BAD_REQUEST")
        {
        }
    }
}