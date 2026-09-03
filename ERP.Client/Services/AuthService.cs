using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using ERP.Application.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace ERP.Client.Services;

public interface IAuthService
{
    bool IsAuthenticated { get; }
    string? AccessToken { get; }
    string? RefreshToken { get; }
    UsuarioDto? CurrentUser { get; }
    Task<bool> LoginAsync(string email, string senha);
    Task<bool> RefreshTokenAsync();
    Task LogoutAsync();
    ClaimsPrincipal GetClaimsPrincipal();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly CustomAuthStateProvider _authStateProvider;
    private string? _accessToken;
    private string? _refreshToken;
    private DateTime _expiration;
    private UsuarioDto? _currentUser;

    public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _expiration;
    public string? AccessToken => _accessToken;
    public string? RefreshToken => _refreshToken;
    public UsuarioDto? CurrentUser => _currentUser;

    public AuthService(HttpClient http, CustomAuthStateProvider authStateProvider)
    {
        _http = http;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> LoginAsync(string email, string senha)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new LoginRequestDto { Email = email, Senha = senha });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                if (result != null)
                {
                    _accessToken = result.AccessToken;
                    _refreshToken = result.RefreshToken;
                    _expiration = result.Expiration;
                    _currentUser = result.Usuario;

                    _authStateProvider.NotifyUserAuthentication(GetClaimsPrincipal());
                    return true;
                }
            }
        }
        catch { }

        // Fallback para Modo Demonstração / GitHub Pages (sem backend servidor ativo)
        if (!string.IsNullOrWhiteSpace(email))
        {
            var perfil = "Administrador";
            var nome = "Administrador Geral";
            var id = 1;

            if (email.ToLower().Contains("vendedor"))
            {
                perfil = "Comercial";
                nome = "João Silva (Vendedor)";
                id = 2;
            }
            else if (email.ToLower().Contains("financeiro"))
            {
                perfil = "Financeiro";
                nome = "Maria Santos (Financeiro)";
                id = 3;
            }

            SetDemoUser(id, nome, email, perfil);
            return true;
        }

        return false;
    }

    private void SetDemoUser(int id, string nome, string email, string perfil)
    {
        _accessToken = "demo_jwt_token_" + Guid.NewGuid().ToString("N");
        _refreshToken = "demo_refresh_token_" + Guid.NewGuid().ToString("N");
        _expiration = DateTime.UtcNow.AddDays(7);
        _currentUser = new UsuarioDto
        {
            Id = id,
            Nome = nome,
            Email = email,
            PerfilId = id,
            PerfilNome = perfil,
            Ativo = true
        };
        _authStateProvider.NotifyUserAuthentication(GetClaimsPrincipal());
    }

    public async Task<bool> RefreshTokenAsync()
    {
        if (string.IsNullOrEmpty(_refreshToken)) return false;

        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/refresh", new RefreshTokenRequestDto
            {
                AccessToken = _accessToken ?? "",
                RefreshToken = _refreshToken
            });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                if (result != null)
                {
                    _accessToken = result.AccessToken;
                    _refreshToken = result.RefreshToken;
                    _expiration = result.Expiration;
                    _currentUser = result.Usuario;

                    _authStateProvider.NotifyUserAuthentication(GetClaimsPrincipal());
                    return true;
                }
            }
        }
        catch { }

        if (_accessToken != null && _accessToken.StartsWith("demo_"))
        {
            _expiration = DateTime.UtcNow.AddDays(7);
            return true;
        }

        await LogoutAsync();
        return false;
    }

    public async Task LogoutAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken) && !_accessToken.StartsWith("demo_"))
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                await _http.SendAsync(req);
            }
            catch { }
        }

        _accessToken = null;
        _refreshToken = null;
        _currentUser = null;
        _authStateProvider.NotifyUserLogout();
    }

    public ClaimsPrincipal GetClaimsPrincipal()
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        if (_accessToken.StartsWith("demo_"))
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _currentUser?.Id.ToString() ?? "1"),
                new Claim(ClaimTypes.Name, _currentUser?.Nome ?? "Administrador Geral"),
                new Claim(ClaimTypes.Email, _currentUser?.Email ?? "admin@azurra.com.br"),
                new Claim(ClaimTypes.Role, _currentUser?.PerfilNome ?? "Administrador")
            };
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "demo"));
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(_accessToken);
            var identity = new ClaimsIdentity(token.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }
        catch
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _currentUser?.Id.ToString() ?? "1"),
                new Claim(ClaimTypes.Name, _currentUser?.Nome ?? "Administrador Geral"),
                new Claim(ClaimTypes.Email, _currentUser?.Email ?? "admin@azurra.com.br"),
                new Claim(ClaimTypes.Role, _currentUser?.PerfilNome ?? "Administrador")
            };
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "demo"));
        }
    }
}

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public void NotifyUserAuthentication(ClaimsPrincipal principal)
    {
        _currentUser = principal;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public void NotifyUserLogout()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }
}
