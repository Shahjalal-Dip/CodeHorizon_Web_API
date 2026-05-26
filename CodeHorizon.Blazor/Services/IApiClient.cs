namespace CodeHorizon.Blazor.Services;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string relativeUrl, CancellationToken ct = default);
    Task<T?> PostAsync<T>(string relativeUrl, object? body = null, CancellationToken ct = default);
    Task<T?> PutAsync<T>(string relativeUrl, object body, CancellationToken ct = default);
    Task<bool> DeleteAsync(string relativeUrl, CancellationToken ct = default);
    Task<T?> PostAuthAsync<T>(string path, object body, CancellationToken ct = default);
    Task<HttpResponseMessage> SendRawAsync(HttpMethod method, string relativeUrl, object? body = null, CancellationToken ct = default);
}
