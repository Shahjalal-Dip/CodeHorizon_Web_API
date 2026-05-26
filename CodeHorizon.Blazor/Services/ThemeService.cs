using CodeHorizon.Blazor.Utils;
using Microsoft.JSInterop;

namespace CodeHorizon.Blazor.Services;

public class ThemeService(ILocalStorageService storage, IJSRuntime js) : IThemeService
{
    public string CurrentTheme { get; private set; } = "light";
    public event Action? OnThemeChanged;

    public async Task InitializeAsync()
    {
        var saved = await storage.GetItemAsync<string>(LocalStorageKeys.Theme);
        CurrentTheme = string.IsNullOrWhiteSpace(saved) ? "light" : saved;
        await ApplyThemeAsync();
    }

    public async Task ToggleThemeAsync()
    {
        CurrentTheme = CurrentTheme == "dark" ? "light" : "dark";
        await storage.SetItemAsync(LocalStorageKeys.Theme, CurrentTheme);
        await ApplyThemeAsync();
        OnThemeChanged?.Invoke();
    }

    public async Task SetThemeAsync(string theme)
    {
        CurrentTheme = theme;
        await storage.SetItemAsync(LocalStorageKeys.Theme, CurrentTheme);
        await ApplyThemeAsync();
        OnThemeChanged?.Invoke();
    }

    private async Task ApplyThemeAsync()
    {
        await js.InvokeVoidAsync("codeHorizon.setTheme", CurrentTheme);
    }
}
