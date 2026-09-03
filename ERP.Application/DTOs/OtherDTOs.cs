using ERP.Domain.Enums;

namespace ERP.Application.DTOs.Comercial
{
    public class OportunidadeCrmDto
{
    public int Id { get; set; }
    public int MailingId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public decimal ValorEstimado { get; set; }
    public StatusCrm Status { get; set; }
    public int ProbabilidadePct { get; set; }
    public DateTime? PrevisaoFechamento { get; set; }
    public string? Responsavel { get; set; }
    public string? Descricao { get; set; }
}

public class PedidoVendaDto
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string? VendedorNome { get; set; }
    public StatusPedido Status { get; set; }
    public DateTime DataEmissao { get; set; }
    public DateTime? DataPrevisaoEntrega { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Desconto { get; set; }
    public decimal ValorFrete { get; set; }
    public decimal ValorTotal { get; set; }
    public List<PedidoVendaItemDto> Itens { get; set; } = new();
}

public class PedidoVendaItemDto
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string ProdutoCodigo { get; set; } = string.Empty;
    public string ProdutoDescricao { get; set; } = string.Empty;
    public string Unidade { get; set; } = "UN";
    public decimal Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal DescontoPct { get; set; }
    public decimal ValorTotal { get; set; }
}
}

namespace ERP.Application.DTOs.Estoque
{
    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Unidade { get; set; } = "UN";
        public string? Ncm { get; set; }
        public string? Grupo { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal PrecoVenda { get; set; }
        public decimal EstoqueAtual { get; set; }
        public decimal EstoqueMinimo { get; set; }
        public bool Ativo { get; set; }
    }

    public class MovimentacaoEstoqueDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string ProdutoDescricao { get; set; } = string.Empty;
        public TipoMovimentacaoEstoque Tipo { get; set; }
        public decimal Quantidade { get; set; }
        public decimal CustoUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public string? DocumentoReferencia { get; set; }
        public string? Motivo { get; set; }
        public DateTime DataMovimento { get; set; }
    }
}

namespace ERP.Application.DTOs.Financeiro
{
    public class TituloFinanceiroDto
    {
        public int Id { get; set; }
        public TipoTitulo Tipo { get; set; }
        public string NumeroDocumento { get; set; } = string.Empty;
        public int? MailingId { get; set; }
        public string? SacadoCedenteNome { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataLiquidacao { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal ValorPagoLiquidado { get; set; }
        public decimal ValorSaldo { get; set; }
        public StatusTitulo Status { get; set; }
        public string? FormaPagamento { get; set; }
        public string? Historico { get; set; }
    }

    public class FluxoCaixaDto
    {
        public decimal SaldoBancos { get; set; }
        public decimal TotalReceberHoje { get; set; }
        public decimal TotalPagarHoje { get; set; }
        public decimal TotalReceberMes { get; set; }
        public decimal TotalPagarMes { get; set; }
        public decimal SaldoProjetadoMes => SaldoBancos + TotalReceberMes - TotalPagarMes;
    }
}

namespace ERP.Application.DTOs.Faturamento
{
    public class NotaFiscalDto
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Serie { get; set; } = "1";
        public string ChaveAcesso { get; set; } = string.Empty;
        public string DestinatarioNome { get; set; } = string.Empty;
        public string DestinatarioCnpjCpf { get; set; } = string.Empty;
        public DateTime DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = "Autorizada";
    }
}

namespace ERP.Application.DTOs.Sistema
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        public string? UsuarioNome { get; set; }
        public string Modulo { get; set; } = string.Empty;
        public string Entidade { get; set; } = string.Empty;
        public string Operacao { get; set; } = string.Empty;
        public string? RegistroId { get; set; }
        public string? Detalhes { get; set; }
    }

    public class EmpresaDto
    {
        public int Id { get; set; }
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string Ie { get; set; } = string.Empty;
        public string? Endereco { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Telefone { get; set; }
        public string SefazAmbiente { get; set; } = "Homologação";
    }
}
