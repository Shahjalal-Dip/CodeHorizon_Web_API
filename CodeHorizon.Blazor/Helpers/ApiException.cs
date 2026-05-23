using CodeHorizon.Blazor.Models.Common;

namespace CodeHorizon.Blazor.Helpers;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public ApiError? Error { get; }

    public ApiException(string message, int statusCode = 500, ApiError? error = null)
        : base(message)
    {
        StatusCode = statusCode;
        Error = error;
    }
}
