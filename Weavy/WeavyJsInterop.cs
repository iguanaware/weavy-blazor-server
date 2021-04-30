using System;
using System.Threading;
using System.Threading.Tasks;
using BlazorApp.Data;
using Microsoft.JSInterop;

namespace BlazorApp.Weavy {
    public class WeavyJsInterop : IDisposable {
        private readonly IJSRuntime js;
        private IJSObjectReference weavyBridge;

        public WeavyJsInterop(IJSRuntime js, TokenService tokenService) {
            this.js = js;
        }

        async public Task Init() {
            weavyBridge = await js.InvokeAsync<IJSObjectReference>("import", "./weavyJsInterop.js");
        }

        async public ValueTask<IJSObjectReference> Weavy(object options = null) {
            return await weavyBridge.InvokeAsync<IJSObjectReference>("weavy", new object[] { options });
        }
    
        async public ValueTask<IJSObjectReference> Space(IJSObjectReference weavy, object spaceSelector = null) {
            return await weavy.InvokeAsync<IJSObjectReference>("space", new object[] { spaceSelector });
        }

        async public ValueTask<IJSObjectReference> App(IJSObjectReference space, object appSelector = null) {
            return await space.InvokeAsync<IJSObjectReference>("app", new object[] { appSelector });
        }

        public void Dispose() {
            weavyBridge?.DisposeAsync();
        }
    }
}

