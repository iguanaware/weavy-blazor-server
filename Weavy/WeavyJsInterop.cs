using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace WeavyBlazorServer.Weavy {
    public class WeavyJsInterop : IDisposable {
        private bool initialized;
        private readonly IJSRuntime _js;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IJSObjectReference _bridge;
        private ValueTask<IJSObjectReference> _whenImport;

        public WeavyJsInterop(IJSRuntime js, IHttpContextAccessor httpContextAccessor) {
            _js = js;
            _httpContextAccessor = httpContextAccessor;

        }
        public async Task Init() {
            if (!initialized) {
                _whenImport = _js.InvokeAsync<IJSObjectReference>("import", "./weavyJsInterop.js");
                _bridge = await _whenImport;
                initialized = true;
            } else {
                await _whenImport;
            }
        }

        public async ValueTask<IJSObjectReference> Weavy(object options = null) {
            await Init();
            // get jwt from current user claims
            var jwt = new { jwt = _httpContextAccessor?.HttpContext?.User?.FindFirst("jwt")?.Value };
            return await _bridge.InvokeAsync<IJSObjectReference>("weavy", new object[] { jwt, options });
        }

        public void Dispose() {
            _bridge?.DisposeAsync();
        }
    }
}

