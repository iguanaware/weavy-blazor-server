using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorApp.Data {
    public class TokenService {

        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IConfiguration _config;

        public TokenService(AuthenticationStateProvider authenticationStateProvider, IConfiguration config) {
            _authenticationStateProvider = authenticationStateProvider;
            _config = config;
        }

        public async Task<string> GetTokenAsync() {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            string token = null;
            if (user.Identity.IsAuthenticated) {
                token = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds()) // valid for 5 minutes
                    .AddClaim("iss", Assembly.GetExecutingAssembly().GetName().Name)
                    .AddClaim("sub", user.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                    .AddClaim("email", user.FindFirst(ClaimTypes.Email)?.Value)
                    .AddClaim("client_id", _config["ClientId"])
                    .WithSecret(_config["ClientSecret"])
                    .Encode();
            }
            return token;
        }
    }
}
