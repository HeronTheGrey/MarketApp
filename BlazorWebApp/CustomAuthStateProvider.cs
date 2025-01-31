using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorWebApp.Authentication
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
        private bool _initialized = false;

        public CustomAuthStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (!_initialized)
            {
                await LoadAuthState();
                _initialized = true;
            }

            return new AuthenticationState(_currentUser);
        }
        
        private async Task LoadAuthState()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

                if (!string.IsNullOrEmpty(token))
                {
                    var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                    _currentUser = new ClaimsPrincipal(identity);
                }
                else
                {
                    _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd pobierania tokena: " + ex.Message);
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        public void NotifyUserAuthentication(string token)
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            _currentUser = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        public void NotifyUserLogout()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string token)
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                Console.WriteLine($"BŁĄD: Niepoprawny JWT: {token}");
                throw new FormatException("JWT token ma niepoprawny format.");
            }

            string payload = parts[1];

            // Normalizacja Base64
            payload = payload.Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            Console.WriteLine($"Dekodowanie JWT payload: {payload}");
    
            try
            {
                var jsonBytes = Convert.FromBase64String(payload);
                var json = Encoding.UTF8.GetString(jsonBytes);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BŁĄD dekodowania JWT: {ex.Message}");
                return new List<Claim>();
            }
        }
    }
}
