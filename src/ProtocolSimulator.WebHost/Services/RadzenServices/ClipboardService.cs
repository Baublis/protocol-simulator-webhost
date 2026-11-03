using Microsoft.JSInterop;

namespace ProtocolSimulator.WebHost.Services.RadzenServices;

public sealed class ClipboardService(IJSRuntime jsRuntime)
{
    public ValueTask WriteTextAsync(string text)
    {
        return jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }
}