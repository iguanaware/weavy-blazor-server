using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace WeavyBlazorServer.Weavy {
    public class WeavyJsInterop : IDisposable {
        private readonly IJSRuntime JS;
        private readonly IHttpContextAccessor HttpContextAccessor;
        private bool Initialized = false;
        private IJSObjectReference Bridge;
        private ValueTask<IJSObjectReference> WhenImport;

        // Constructor
        // This is a good place to inject any authentication service you may use to provide JWT tokens.
        public WeavyJsInterop(IJSRuntime js, IHttpContextAccessor httpContextAccessor) {
            JS = js;
            HttpContextAccessor = httpContextAccessor;
        }

        // Initialization of the JS Interop Module
        // The initialization is only done once even if you call it multiple times
        public async Task Init() {
            if (!Initialized) {
                Initialized = true;
                WhenImport = JS.InvokeAsync<IJSObjectReference>("import", "./weavyJsInterop.js");
                Bridge = await WhenImport;
            } else {
                await WhenImport;
            }
        }

        // Calling Javascript to create a new instance of Weavy via the JS Interop Module
        public async ValueTask<IJSObjectReference> Weavy(object options = null) {
            await Init();
            // Configure your own JWT here
            // Currently using Demo JWT only for showcase.weavycloud.com
            // Get jwt from current user claims
            var jwt = new { jwt = HttpContextAccessor?.HttpContext?.User?.FindFirst("jwt")?.Value };
            return await Bridge.InvokeAsync<IJSObjectReference>("weavy", new object[] { jwt, options });
        }

        public void Dispose() {
            Bridge?.DisposeAsync();
        }
    }
}

