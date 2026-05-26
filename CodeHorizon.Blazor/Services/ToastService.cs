namespace CodeHorizon.Blazor.Services;

public class ToastService : IToastService
{
    public event Action<ToastMessage>? OnShow;

    public void ShowSuccess(string message) => Show(message, ToastType.Success);
    public void ShowError(string message) => Show(message, ToastType.Error);
    public void ShowInfo(string message) => Show(message, ToastType.Info);
    public void ShowWarning(string message) => Show(message, ToastType.Warning);

    private void Show(string message, ToastType type) =>
        OnShow?.Invoke(new ToastMessage(message, type));
}
