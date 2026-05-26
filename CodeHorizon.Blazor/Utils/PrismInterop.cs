using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CodeHorizon.Blazor.Utils;

public class PrismInterop(IJSRuntime js)
{
    public ValueTask HighlightAllAsync() =>
        js.InvokeVoidAsync("Prism.highlightAll");

    public ValueTask HighlightElementAsync(ElementReference element) =>
        js.InvokeVoidAsync("Prism.highlightElement", element);
}
