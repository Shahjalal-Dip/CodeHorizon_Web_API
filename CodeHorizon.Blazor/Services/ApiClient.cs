using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CodeHorizon.Blazor.Helpers;
using CodeHorizon.Blazor.Models.Common;
using CodeHorizon.Blazor.Models.Config;
using CodeHorizon.Blazor.Providers;
using CodeHorizon.Blazor.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace CodeHorizon.Blazor.Services;

public class ApiClient(
    HttpClient http,
    IOptions<ApiSettings> apiOptions,
    NavigationManager navigation,
    ILocalStorageService storage,
    CustomAuthStateProvider authStateProvider,
    ILogger<ApiClient> logger) : IApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly ApiSettings _settings = apiOptions.Value;

    public Task<T?> GetAsync<T>(string relativeUrl, CancellationToken ct = default) =>
        SendAsync<T>(HttpMethod.Get, relativeUrl, null, ct);

    public Task<T?> PostAsync<T>(string relativeUrl, object? body = null, CancellationToken ct = default) =>
        SendAsync<T>(HttpMethod.Post, relativeUrl, body, ct);

    public Task<T?> PutAsync<T>(string relativeUrl, object body, CancellationToken ct = default) =>
        SendAsync<T>(HttpMethod.Put, relativeUrl, body, ct);

    public async Task<bool> DeleteAsync(string relativeUrl, CancellationToken ct = default)
    {
        var response = await SendRawAsync(HttpMethod.Delete, relativeUrl, null, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<T?> PostAuthAsync<T>(string path, object body, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.AuthBase}/{path.TrimStart('/')}")
        {
            Content = JsonContent.Create(body, options: JsonOptions)
        };

        var response = await http.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(ct);
            ApiError? error = null;
            try { error = JsonSerializer.Deserialize<ApiError>(responseBody, JsonOptions); }
            catch { /* ignore */ }

            if (error is null)
            {
                try
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    if (doc.RootElement.TryGetProperty("message", out var msg))
                        throw new ApiException(msg.GetString() ?? "Authentication failed.", (int)response.StatusCode);
                }
                catch (ApiException) { throw; }
                catch { /* ignore */ }
            }

            throw new ApiException(error?.FriendlyMessage ?? "Authentication failed.", (int)response.StatusCode, error);
        }

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct);
    }

    public Task<HttpResponseMessage> SendRawAsync(HttpMethod method, string relativeUrl, object? body = null, CancellationToken ct = default)
    {
        var request = CreateRequest(method, relativeUrl, body);
        return http.SendAsync(request, ct);
    }

    private async Task<T?> SendAsync<T>(HttpMethod method, string relativeUrl, object? body, CancellationToken ct)
    {
        using var request = CreateRequest(method, relativeUrl, body);
        var response = await http.SendAsync(request, ct);
        return await HandleResponseAsync<T>(response, ct);
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string relativeUrl, object? body)
    {
        var request = new HttpRequestMessage(method, relativeUrl.TrimStart('/'));
        if (body is not null)
            request.Content = JsonContent.Create(body, options: JsonOptions);
        return request;
    }

    private async Task<T?> HandleResponseAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await storage.RemoveItemAsync(LocalStorageKeys.AuthToken);
            await authStateProvider.MarkUserAsLoggedOut();
            navigation.NavigateTo($"/login?returnUrl={Uri.EscapeDataString(navigation.Uri)}");
            throw new ApiException("Your session has expired. Please sign in again.", 401);
        }

        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NoContent)
                return default;

            if (typeof(T) == typeof(object) || typeof(T) == typeof(bool))
                return default;

            return await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct);
        }

        ApiError? error = null;
        try
        {
            error = await response.Content.ReadFromJsonAsync<ApiError>(JsonOptions, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to parse API error response");
        }

        var message = error?.FriendlyMessage ?? $"Request failed ({(int)response.StatusCode})";
        logger.LogWarning("API error {Status}: {Message}", response.StatusCode, message);
        throw new ApiException(message, (int)response.StatusCode, error);
    }
}
