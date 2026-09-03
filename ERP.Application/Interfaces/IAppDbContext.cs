using ERP.Domain.Entities.Auth;
using ERP.Domain.Entities.Comercial;
using ERP.Domain.Entities.Estoque;
using ERP.Domain.Entities.Faturamento;
using ERP.Domain.Entities.Financeiro;
using ERP.Domain.Entities.Mailings;
using ERP.Domain.Entities.Sistema;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<Usuario> Usuarios { get; }
    DbSet<Perfil> Perfis { get; }
    DbSet<PerfilModulo> PerfilModulos { get; }
    DbSet<PerfilPermissao> PerfilPermissoes { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    DbSet<Mailing> Mailings { get; }
    DbSet<MailingEndereco> MailingEnderecos { get; }
    DbSet<MailingContato> MailingContatos { get; }
    DbSet<MailingPreferencia> MailingPreferencias { get; }
    DbSet<MailingCnae> MailingCnaes { get; }
    DbSet<MailingAcao> MailingAcoes { get; }
    DbSet<MailingFollowUp> MailingFollowUps { get; }
    DbSet<MailingDadoBancario> MailingDadosBancarios { get; }
    DbSet<MailingVeiculo> MailingVeiculos { get; }
    DbSet<MailingRegiao> MailingRegioes { get; }
    DbSet<MailingDocumento> MailingDocumentos { get; }
    DbSet<MailingFaturamento> MailingFaturamentos { get; }

    DbSet<OportunidadeCrm> OportunidadesCrm { get; }
    DbSet<OrdemServico> OrdensServico { get; }
    DbSet<PedidoVenda> PedidosVenda { get; }
    DbSet<PedidoVendaItem> PedidoVendaItens { get; }

    DbSet<Produto> Produtos { get; }
    DbSet<LocalEstoque> LocaisEstoque { get; }
    DbSet<Lote> Lotes { get; }
    DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; }

    DbSet<ContaBancaria> ContasBancarias { get; }
    DbSet<CentroCusto> CentrosCusto { get; }
    DbSet<PlanoConta> PlanosContas { get; }
    DbSet<TituloFinanceiro> TitulosFinanceiros { get; }

    DbSet<TributacaoConfig> TributacoesConfig { get; }
    DbSet<NotaFiscal> NotasFiscais { get; }
    DbSet<NotaFiscalItem> NotaFiscalItens { get; }

    DbSet<AuditLog> AuditLogs { get; }
    DbSet<Empresa> Empresas { get; }
    DbSet<ParametrosSistema> Parametros { get; }
    DbSet<Feriado> Feriados { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface ICurrentUserService
{
    int? UsuarioId { get; }
    string? UsuarioNome { get; }
    string? Email { get; }
    string? PerfilNome { get; }
    bool IsAuthenticated { get; }
}

public interface ITokenService
{
    (string token, DateTime expiration) GenerateAccessToken(Usuario usuario);
    string GenerateRefreshToken();
}

public interface IExternalLookupService
{
    Task<EnderecoViaCepResult?> BuscarCepAsync(string cep);
    Task<CnpjReceitaResult?> BuscarCnpjAsync(string cnpj);
}

public class EnderecoViaCepResult
{
    public string Cep { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Localidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
}

public class CnpjReceitaResult
{
    public string Cnpj { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Fantasia { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Situacao { get; set; } = string.Empty;
}
