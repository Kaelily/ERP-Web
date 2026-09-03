using ERP.Domain.Entities.Common;
using ERP.Domain.Entities.Mailings;
using ERP.Domain.Enums;

namespace ERP.Domain.Entities.Financeiro;

public class ContaBancaria : BaseEntity
{
    public string NomeConta { get; set; } = string.Empty;
    public string BancoCodigo { get; set; } = string.Empty;
    public string BancoNome { get; set; } = string.Empty;
    public string Agencia { get; set; } = string.Empty;
    public string NumeroConta { get; set; } = string.Empty;
    public decimal SaldoAtual { get; set; }
    public bool Ativo { get; set; } = true;
}

public class CentroCusto : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}

public class PlanoConta : BaseEntity
{
    public string CodigoEstruturado { get; set; } = string.Empty; // ex: 1.1.01.001
    public string Descricao { get; set; } = string.Empty;
    public string Tipo { get; set; } = "D"; // D (Débito) ou C (Crédito)
    public bool Sintetica { get; set; } = false;
    public bool Ativo { get; set; } = true;
}

public class TituloFinanceiro : BaseEntity, IAuditableEntity
{
    public TipoTitulo Tipo { get; set; } = TipoTitulo.Receber;
    public string NumeroDocumento { get; set; } = string.Empty;
    public int? MailingId { get; set; }
    public Mailing? Mailing { get; set; }
    public string? SacadoCedenteNome { get; set; }
    public int? ContaBancariaId { get; set; }
    public ContaBancaria? ContaBancaria { get; set; }
    public int? CentroCustoId { get; set; }
    public CentroCusto? CentroCusto { get; set; }

    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
    public DateTime DataVencimento { get; set; }
    public DateTime? DataLiquidacao { get; set; }

    public decimal ValorOriginal { get; set; }
    public decimal ValorDesconto { get; set; }
    public decimal ValorJurosMulta { get; set; }
    public decimal ValorPagoLiquidado { get; set; }
    public decimal ValorSaldo { get; set; }
    
    public StatusTitulo Status { get; set; } = StatusTitulo.Pendente;
    public string? FormaPagamento { get; set; } // Boleto, PIX, TED, Cartão
    public string? Historico { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }
}
