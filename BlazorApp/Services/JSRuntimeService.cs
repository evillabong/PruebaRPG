using Microsoft.JSInterop;

namespace BlazorApp.Services
{
    public static class JSRuntimeService
    {
        public static ValueTask Alert (this IJSRuntime js, string message)
        {
            return js.InvokeVoidAsync("alert", message);
        }
        public static ValueTask Error (this IJSRuntime js, string message)
        {
            return js.InvokeVoidAsync("console.error", message);
        }
        public static ValueTask Log (this IJSRuntime js, string message)
        {
            return js.InvokeVoidAsync("console.log", message);
        }
        public static ValueTask Warn (this IJSRuntime js, string message)
        {
            return js.InvokeVoidAsync("console.warn", message);
        }
    }
}
