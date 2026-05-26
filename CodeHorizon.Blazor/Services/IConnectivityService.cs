namespace CodeHorizon.Blazor.Services;

public interface IConnectivityService
{
    bool IsOnline { get; }
    event Action<bool>? OnConnectivityChanged;
    Task InitializeAsync();
}
