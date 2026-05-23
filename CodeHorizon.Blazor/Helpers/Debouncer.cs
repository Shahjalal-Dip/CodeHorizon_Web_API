namespace CodeHorizon.Blazor.Helpers;

public class Debouncer
{
    private CancellationTokenSource? _cts;

    public async Task DebounceAsync(Func<Task> action, int delayMs = 300)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            await Task.Delay(delayMs, token);
            if (!token.IsCancellationRequested)
                await action();
        }
        catch (TaskCanceledException)
        {
            // ignored
        }
    }
}
