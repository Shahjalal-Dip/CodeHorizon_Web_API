namespace CodeHorizon.Blazor.Services;

public interface IToastService
{
    event Action<ToastMessage>? OnShow;
    void ShowSuccess(string message);
    void ShowError(string message);
    void ShowInfo(string message);
    void ShowWarning(string message);
}

public record ToastMessage(string Message, ToastType Type, int DurationMs = 4000);

public enum ToastType { Success, Error, Info, Warning }
