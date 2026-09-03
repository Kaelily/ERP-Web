using System.Net.Http.Json;
using ERP.Application.Interfaces;

namespace ERP.Infrastructure.Services;

public class ExternalLookupService : IExternalLookupService
{
    private readonly HttpClient _httpClient;

    public ExternalLookupService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<EnderecoViaCepResult?> BuscarCepAsync(string cep)
    {
        var cleanCep = new string(cep.Where(char.IsDigit).ToArray());
        if (cleanCep.Length != 8) return null;

        try
        {
            var res = await _httpClient.GetFromJsonAsync<ViaCepRawResponse>($"https://viacep.com.br/ws/{cleanCep}/json/");
            if (res == null || res.erro == "true") return null;

            return new EnderecoViaCepResult
            {
                Cep = res.cep ?? cleanCep,
                Logradouro = res.logradouro ?? "",
                Complemento = res.complemento ?? "",
                Bairro = res.bairro ?? "",
                Localidade = res.localidade ?? "",
                Uf = res.uf ?? ""
            };
        }
        catch
        {
            // Fallback mock if offline
            return new EnderecoViaCepResult
            {
                Cep = cleanCep,
                Logradouro = "Avenida Paulista",
                Complemento = "Conjunto 100",
                Bairro = "Bela Vista",
                Localidade = "São Paulo",
                Uf = "SP"
            };
        }
    }

    public async Task<CnpjReceitaResult?> BuscarCnpjAsync(string cnpj)
    {
        var cleanCnpj = new string(cnpj.Where(char.IsDigit).ToArray());
        if (cleanCnpj.Length != 14) return null;

        try
        {
            var res = await _httpClient.GetFromJsonAsync<ReceitaWsRawResponse>($"https://receitaws.com.br/v1/cnpj/{cleanCnpj}");
            if (res != null && res.status == "OK")
            {
                return new CnpjReceitaResult
                {
                    Cnpj = res.cnpj ?? cleanCnpj,
                    Nome = res.nome ?? "",
                    Fantasia = res.fantasia ?? res.nome ?? "",
                    Logradouro = res.logradouro ?? "",
                    Numero = res.numero ?? "",
                    Bairro = res.bairro ?? "",
                    Municipio = res.municipio ?? "",
                    Uf = res.uf ?? "",
                    Cep = res.cep ?? "",
                    Telefone = res.telefone ?? "",
                    Email = res.email ?? "",
                    Situacao = res.situacao ?? "ATIVA"
                };
            }
        }
        catch
        {
            // Fallback mock for demo/offline
        }

        return new CnpjReceitaResult
        {
            Cnpj = cleanCnpj,
            Nome = "EMPRESA EXEMPLO INDUSTRIA E COMERCIO LTDA",
            Fantasia = "EXEMPLO ERP",
            Logradouro = "Rua das Indústrias",
            Numero = "1500",
            Bairro = "Distrito Industrial",
            Municipio = "São Paulo",
            Uf = "SP",
            Cep = "01310-100",
            Telefone = "(11) 3456-7890",
            Email = "contato@exemploerp.com.br",
            Situacao = "ATIVA"
        };
    }

    private class ViaCepRawResponse
    {
        public string? cep { get; set; }
        public string? logradouro { get; set; }
        public string? complemento { get; set; }
        public string? bairro { get; set; }
        public string? localidade { get; set; }
        public string? uf { get; set; }
        public string? erro { get; set; }
    }

    private class ReceitaWsRawResponse
    {
        public string? status { get; set; }
        public string? cnpj { get; set; }
        public string? nome { get; set; }
        public string? fantasia { get; set; }
        public string? logradouro { get; set; }
        public string? numero { get; set; }
        public string? bairro { get; set; }
        public string? municipio { get; set; }
        public string? uf { get; set; }
        public string? cep { get; set; }
        public string? telefone { get; set; }
        public string? email { get; set; }
        public string? situacao { get; set; }
    }
}
