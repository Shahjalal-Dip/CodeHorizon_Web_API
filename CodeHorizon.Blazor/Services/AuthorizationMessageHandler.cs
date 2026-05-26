using System.Net.Http.Headers;
using CodeHorizon.Blazor.Utils;

namespace CodeHorizon.Blazor.Services;

public class AuthorizationMessageHandler(ILocalStorageService storage) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await storage.GetItemAsync<string>(LocalStorageKeys.AuthToken);
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
