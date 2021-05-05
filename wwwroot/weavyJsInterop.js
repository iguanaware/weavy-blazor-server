console.log("weavyJsInterop.js");

export function weavy(...options) {
    var jwtOptions = { jwt: () => DotNet.invokeMethodAsync('BlazorApp', 'GetJwt') };
    return new window.Weavy(jwtOptions, ...options);
}