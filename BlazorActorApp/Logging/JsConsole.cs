﻿using Microsoft.JSInterop;

namespace BlazorActorApp.Logging
{
    public class JsConsole
    {
        private readonly IJSRuntime _jsRuntime;
        public JsConsole(IJSRuntime jSRuntime)
        {
            _jsRuntime = jSRuntime;
        }

        public async Task LogAsync(string message)
        {
            await _jsRuntime.InvokeVoidAsync("console.log", message);
        }

        public async Task ClearAsync()
        {
            await _jsRuntime.InvokeVoidAsync("console.clear");
        }
    }
}
