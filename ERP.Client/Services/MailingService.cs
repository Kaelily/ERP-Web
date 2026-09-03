using System.Net.Http.Json;
using ERP.Application.Common;
using ERP.Application.DTOs.Mailing;
using ERP.Application.Interfaces;

namespace ERP.Client.Services;

public interface IMailingService
{
    Task<PagedResult<MailingListItemDto>> GetListAsync(MailingFilterDto filter);
    Task<MailingDto?> GetByIdAsync(int id);
    Task<int?> CreateAsync(MailingCreateDto dto);
    Task<bool> UpdateAsync(MailingUpdateDto dto);
    Task<bool> InativarAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<MailingEstatisticasDto?> GetEstatisticasAsync(int id);
    Task<EnderecoViaCepResult?> BuscarCepAsync(string cep);
    Task<CnpjReceitaResult?> BuscarCnpjAsync(string cnpj);
}

public class MailingService : IMailingService
{
    private readonly ErpApiClient _api;
    private readonly HttpClient _http;

    public MailingService(ErpApiClient api, HttpClient http)
    {
        _api = api;
        _http = http;
    }

    public async Task<PagedResult<MailingListItemDto>> GetListAsync(MailingFilterDto filter)
    {
        var queryParams = new List<string>
        {
            $"pageIndex={filter.PageIndex}",
            $"pageSize={filter.PageSize}"
        };

        if (!string.IsNullOrWhiteSpace(filter.Termo)) queryParams.Add($"termo={Uri.EscapeDataString(filter.Termo)}");
        if (!string.IsNullOrWhiteSpace(filter.Tipo)) queryParams.Add($"tipo={Uri.EscapeDataString(filter.Tipo)}");
        if (filter.VendedorId.HasValue) queryParams.Add($"vendedorId={filter.VendedorId.Value}");
        if (filter.Inativo.HasValue) queryParams.Add($"inativo={filter.Inativo.Value}");
        if (filter.Bloqueado.HasValue) queryParams.Add($"bloqueado={filter.Bloqueado.Value}");
        if (!string.IsNullOrWhiteSpace(filter.SortBy)) queryParams.Add($"sortBy={filter.SortBy}&sortDescending={filter.SortDescending}");

        var url = $"api/mailing?{string.Join("&", queryParams)}";
        var result = await _api.GetAsync<PagedResult<MailingListItemDto>>(url);
        return result ?? new PagedResult<MailingListItemDto>();
    }

    public async Task<MailingDto?> GetByIdAsync(int id)
    {
        return await _api.GetAsync<MailingDto>($"api/mailing/{id}");
    }

    public async Task<int?> CreateAsync(MailingCreateDto dto)
    {
        var res = await _api.PostAsync("api/mailing", dto);
        if (res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadFromJsonAsync<CreateResult>();
            return body?.Id;
        }
        return null;
    }

    public async Task<bool> UpdateAsync(MailingUpdateDto dto)
    {
        var res = await _api.PutAsync($"api/mailing/{dto.Id}", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> InativarAsync(int id)
    {
        var res = await _api.PatchAsync($"api/mailing/{id}/inativar");
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var res = await _api.DeleteAsync($"api/mailing/{id}");
        return res.IsSuccessStatusCode;
    }

    public async Task<MailingEstatisticasDto?> GetEstatisticasAsync(int id)
    {
        return await _api.GetAsync<MailingEstatisticasDto>($"api/mailing/{id}/estatisticas");
    }

    public async Task<EnderecoViaCepResult?> BuscarCepAsync(string cep)
    {
        return await _api.GetAsync<EnderecoViaCepResult>($"api/mailing/lookup/cep/{cep}");
    }

    public async Task<CnpjReceitaResult?> BuscarCnpjAsync(string cnpj)
    {
        return await _api.GetAsync<CnpjReceitaResult>($"api/mailing/lookup/cnpj/{cnpj}");
    }

    private class CreateResult
    {
        public int Id { get; set; }
        public string? Message { get; set; }
    }
}
