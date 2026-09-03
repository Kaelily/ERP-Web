using System.Net.Http.Json;
using ERP.Application.DTOs.Comercial;
using ERP.Application.DTOs.Estoque;
using ERP.Application.DTOs.Financeiro;
using ERP.Application.DTOs.Sistema;
using ERP.Domain.Enums;

namespace ERP.Client.Services;

public class ComercialService
{
    private readonly ErpApiClient _api;

    public ComercialService(ErpApiClient api)
    {
        _api = api;
    }

    public async Task<List<OportunidadeCrmDto>> GetOportunidadesAsync()
    {
        return await _api.GetAsync<List<OportunidadeCrmDto>>("api/comercial/crm/oportunidades") ?? new();
    }

    public async Task<bool> CreateOportunidadeAsync(OportunidadeCrmDto dto)
    {
        var res = await _api.PostAsync("api/comercial/crm/oportunidades", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateStatusOportunidadeAsync(int id, StatusCrm status)
    {
        var res = await _api.PatchAsync($"api/comercial/crm/oportunidades/{id}/status");
        return res.IsSuccessStatusCode;
    }

    public async Task<List<PedidoVendaDto>> GetPedidosAsync()
    {
        return await _api.GetAsync<List<PedidoVendaDto>>("api/comercial/pedidos") ?? new();
    }
}

public class EstoqueService
{
    private readonly ErpApiClient _api;

    public EstoqueService(ErpApiClient api)
    {
        _api = api;
    }

    public async Task<List<ProdutoDto>> GetProdutosAsync()
    {
        return await _api.GetAsync<List<ProdutoDto>>("api/estoque/produtos") ?? new();
    }

    public async Task<bool> CreateProdutoAsync(ProdutoDto dto)
    {
        var res = await _api.PostAsync("api/estoque/produtos", dto);
        return res.IsSuccessStatusCode;
    }
}

public class FinanceiroService
{
    private readonly ErpApiClient _api;

    public FinanceiroService(ErpApiClient api)
    {
        _api = api;
    }

    public async Task<List<TituloFinanceiroDto>> GetTitulosAsync(TipoTitulo? tipo = null)
    {
        var url = tipo.HasValue ? $"api/financeiro/titulos?tipo={tipo.Value}" : "api/financeiro/titulos";
        return await _api.GetAsync<List<TituloFinanceiroDto>>(url) ?? new();
    }

    public async Task<FluxoCaixaDto?> GetFluxoCaixaAsync()
    {
        return await _api.GetAsync<FluxoCaixaDto>("api/financeiro/fluxo-caixa");
    }
}

public class SistemaService
{
    private readonly ErpApiClient _api;

    public SistemaService(ErpApiClient api)
    {
        _api = api;
    }

    public async Task<List<AuditLogDto>> GetAuditLogsAsync()
    {
        return await _api.GetAsync<List<AuditLogDto>>("api/sistema/audit-logs") ?? new();
    }

    public async Task<EmpresaDto?> GetEmpresaAsync()
    {
        return await _api.GetAsync<EmpresaDto>("api/sistema/empresa");
    }
}
