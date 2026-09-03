using ERP.Domain.Entities.Common;
using ERP.Domain.Entities.Mailings;

namespace ERP.Domain.Entities.Faturamento;

public class TributacaoConfig : BaseEntity
{
    public string Ncm { get; set; } = string.Empty;
    public string Cfop { get; set; } = "5102";
    public decimal AliquotaIcms { get; set; }
    public decimal AliquotaPis { get; set; }
    public decimal AliquotaCofins { get; set; }
    public decimal AliquotaIpi { get; set; }
    public decimal AliquotaIbs { get; set; } // Reforma Tributária 2026
    public decimal AliquotaCbs { get; set; } // Reforma Tributária 2026
    public string? Descricao { get; set; }
}

public class NotaFiscal : BaseEntity, IAuditableEntity
{
    public string Numero { get; set; } = string.Empty;
    public string Serie { get; set; } = "1";
    public string ChaveAcesso { get; set; } = string.Empty;
    public string Modelo { get; set; } = "55"; // 55 = NF-e, 65 = NFC-e
    public int? DestinatarioId { get; set; }
    public Mailing? Destinatario { get; set; }
    public string DestinatarioNome { get; set; } = string.Empty;
    public string DestinatarioCnpjCpf { get; set; } = string.Empty;
    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
    public DateTime? DataAutorizacao { get; set; }
    public decimal ValorProdutos { get; set; }
    public decimal ValorFrete { get; set; }
    public decimal ValorImpostos { get; set; }
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = "Autorizada"; // Emitida, Autorizada, Cancelada, Inutilizada, Rejeitada
    public string? MotivoStatus { get; set; }
    public string? XmlAssinado { get; set; }

    public ICollection<NotaFiscalItem> Itens { get; set; } = new List<NotaFiscalItem>();

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }
}

public class NotaFiscalItem : BaseEntity
{
    public int NotaFiscalId { get; set; }
    public NotaFiscal NotaFiscal { get; set; } = null!;
    public int ProdutoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Ncm { get; set; } = string.Empty;
    public string Cfop { get; set; } = "5102";
    public string Unidade { get; set; } = "UN";
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal ValorIcms { get; set; }
    public decimal ValorPis { get; set; }
    public decimal ValorCofins { get; set; }
}
