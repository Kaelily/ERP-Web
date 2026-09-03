using System.Net.Http.Headers;
using System.Net.Http.Json;
using ERP.Application.Common;
using MudBlazor;

namespace ERP.Client.Services;

public class ErpApiClient
{
    private readonly HttpClient _http;
    private readonly IAuthService _auth;
    private readonly ISnackbar _snackbar;

    public ErpApiClient(HttpClient http, IAuthService auth, ISnackbar snackbar)
    {
        _http = http;
        _auth = auth;
        _snackbar = snackbar;
    }

    private void AddAuthHeader(HttpRequestMessage request)
    {
        if (_auth.IsAuthenticated && !string.IsNullOrEmpty(_auth.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.AccessToken);
        }
    }

    public async Task<T?> GetAsync<T>(string uri)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            AddAuthHeader(request);

            var response = await _http.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await _auth.RefreshTokenAsync();
                if (refreshed)
                {
                    request = new HttpRequestMessage(HttpMethod.Get, uri);
                    AddAuthHeader(request);
                    response = await _http.SendAsync(request);
                }
            }

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            await HandleErrorResponse(response);
            return default;
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Erro de conexão: {ex.Message}", Severity.Error);
            return default;
        }
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string uri, T value)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(value)
            };
            AddAuthHeader(request);

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponse(response);
            }
            return response;
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Erro ao salvar dados: {ex.Message}", Severity.Error);
            throw;
        }
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string uri, T value)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri)
            {
                Content = JsonContent.Create(value)
            };
            AddAuthHeader(request);

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponse(response);
            }
            return response;
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Erro ao atualizar dados: {ex.Message}", Severity.Error);
            throw;
        }
    }

    public async Task<HttpResponseMessage> PatchAsync(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, uri);
        AddAuthHeader(request);
        var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponse(response);
        }
        return response;
    }

    public async Task<HttpResponseMessage> DeleteAsync(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, uri);
        AddAuthHeader(request);
        var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponse(response);
        }
        return response;
    }

    private async Task HandleErrorResponse(HttpResponseMessage response)
    {
        try
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Silently ignore 404/401 errors for demo mode / GitHub Pages hosting
                return;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            {
                var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsDto>();
                if (problem?.Errors != null && problem.Errors.Any())
                {
                    foreach (var errList in problem.Errors.Values)
                    {
                        foreach (var err in errList)
                        {
                            _snackbar.Add(err, Severity.Warning);
                        }
                    }
                    return;
                }
            }

            var raw = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(raw))
            {
                _snackbar.Add($"Operação não concluída ({response.StatusCode}): {raw}", Severity.Error);
            }
        }
        catch { }
    }
}
