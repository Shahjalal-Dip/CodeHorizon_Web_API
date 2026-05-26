using Microsoft.JSInterop;

namespace CodeHorizon.Blazor.Services;

public class ConnectivityService(IJSRuntime js) : IConnectivityService, IAsyncDisposable
{
    private DotNetObjectReference<ConnectivityService>? _ref;

    public bool IsOnline { get; private set; } = true;
    public event Action<bool>? OnConnectivityChanged;

    public async Task InitializeAsync()
    {
        _ref = DotNetObjectReference.Create(this);
        IsOnline = await js.InvokeAsync<bool>("codeHorizon.getOnlineStatus");
        await js.InvokeVoidAsync("codeHorizon.registerConnectivity", _ref);
    }

    [JSInvokable]
    public void SetOnlineStatus(bool isOnline)
    {
        IsOnline = isOnline;
        OnConnectivityChanged?.Invoke(isOnline);
    }

    public ValueTask DisposeAsync()
    {
        _ref?.Dispose();
        return ValueTask.CompletedTask;
    }
}
