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
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (result == null) return false;

            _accessToken = result.AccessToken;
            _refreshToken = result.RefreshToken;
            _expiration = result.Expiration;
            _currentUser = result.Usuario;

            _authStateProvider.NotifyUserAuthentication(GetClaimsPrincipal());
            return true;
        }
        catch
        {
            return false;
        }
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

            if (!response.IsSuccessStatusCode)
            {
                await LogoutAsync();
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (result == null) return false;

            _accessToken = result.AccessToken;
            _refreshToken = result.RefreshToken;
            _expiration = result.Expiration;
            _currentUser = result.Usuario;

            _authStateProvider.NotifyUserAuthentication(GetClaimsPrincipal());
            return true;
        }
        catch
        {
            await LogoutAsync();
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken))
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

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(_accessToken);
        var identity = new ClaimsIdentity(token.Claims, "jwt");

        return new ClaimsPrincipal(identity);
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
