using System;
using System.Threading.Tasks;
using BlazorApp.Data;
using Microsoft.JSInterop;

namespace BlazorApp.Weavy {
    public class WeavyJsInterop : IDisposable {
        private readonly IJSRuntime js;
        public IJSObjectReference bridge;

        public WeavyJsInterop(IJSRuntime js, TokenService tokenService) {
            this.js = js;
        }

        async public Task Init() {
            bridge = await js.InvokeAsync<IJSObjectReference>("import", "./weavyJsInterop.js");
        }

        async public ValueTask<WeavyReference> Weavy(object options = null) {
            var _weavy = await bridge.InvokeAsync<IJSObjectReference>("weavy", new object[] { options });
            return new WeavyReference(_weavy);
        }

        public void Dispose() {
            bridge?.DisposeAsync();
        }
    }

    public class WeavyReference : ExtendableJSObjectReference {
        public WeavyReference(IJSObjectReference weavy) : base(weavy) {}

        async public ValueTask<SpaceReference> Space(object spaceSelector = null) {
            var space = await objectReference.InvokeAsync<IJSObjectReference>("space", new object[] { spaceSelector });
            return new SpaceReference(space);
        }
    }

    public class SpaceReference : ExtendableJSObjectReference {
        public SpaceReference(IJSObjectReference space) : base(space) { }

        async public ValueTask<AppReference> App(object appSelector = null) {
            var app = await objectReference.InvokeAsync<IJSObjectReference>("app", new object[] { appSelector });
            return new AppReference(app);
        }
    }

    public class AppReference : ExtendableJSObjectReference {
        public AppReference(IJSObjectReference app) : base(app) { }

        public ValueTask<IJSObjectReference> Open() {
            return objectReference.InvokeAsync<IJSObjectReference>("open", new object[] { });
        }
        
        public ValueTask<IJSObjectReference> Close() {
            return objectReference.InvokeAsync<IJSObjectReference>("close", new object[] { });
        }

        public ValueTask<IJSObjectReference> Toggle() {
            return objectReference.InvokeAsync<IJSObjectReference>("toggle", new object[] { });
        }
    }
}

