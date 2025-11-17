using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;

namespace FrontendBlazor.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private const string TOKEN_KEY = "authToken";
        private readonly ILocalStorageService _localStorage;
        private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsStringAsync(TOKEN_KEY);
                if (string.IsNullOrWhiteSpace(token))
                    return new AuthenticationState(_anonymous);  

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                if (jwt.ValidTo < DateTime.UtcNow)
                {
                    await LogoutAsync();
                    return new AuthenticationState(_anonymous);
                }

                var user = CreateClaimsPrincipalFromToken(token);
                return new AuthenticationState(user);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task LoginAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            await _localStorage.SetItemAsStringAsync(TOKEN_KEY, token);
            var user = CreateClaimsPrincipalFromToken(token);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        // Método para logout
        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync(TOKEN_KEY);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }


        private ClaimsPrincipal CreateClaimsPrincipalFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                if (jwt.ValidTo < DateTime.UtcNow)
                    return _anonymous;

                var identity = new ClaimsIdentity(jwt.Claims, "jwt");

                var tipoUsuario = jwt.Claims.FirstOrDefault(c => c.Type == "tipoUsuario")?.Value;
                if (!string.IsNullOrEmpty(tipoUsuario))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, tipoUsuario));
                }

                return new ClaimsPrincipal(identity);
            }
            catch
            {
                return _anonymous;
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsStringAsync(TOKEN_KEY);
        }

        public async Task<string?> GetNomeAsync()
        {
            var state = await GetAuthenticationStateAsync();
            return state.User.Identity?.IsAuthenticated == true
                ? state.User.FindFirst("nome")?.Value
                : null;
        }

    }
}