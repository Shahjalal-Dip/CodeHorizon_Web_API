using Microsoft.JSInterop;

namespace CodeHorizon.Blazor.Utils;

public class ClipboardInterop(IJSRuntime js)
{
    public ValueTask CopyAsync(string text) =>
        js.InvokeVoidAsync("codeHorizon.copyToClipboard", text);

    public ValueTask ShareAsync(string title, string text, string url) =>
        js.InvokeVoidAsync("codeHorizon.share", title, text, url);
}
