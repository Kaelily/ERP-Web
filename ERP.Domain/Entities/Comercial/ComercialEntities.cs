using ERP.Domain.Entities.Common;
using ERP.Domain.Entities.Mailings;
using ERP.Domain.Enums;

namespace ERP.Domain.Entities.Comercial;

public class OportunidadeCrm : BaseEntity, IAuditableEntity
{
    public int MailingId { get; set; }
    public Mailing? Mailing { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public decimal ValorEstimado { get; set; }
    public StatusCrm Status { get; set; } = StatusCrm.Prospeccao;
    public int ProbabilidadePct { get; set; } = 20;
    public DateTime? PrevisaoFechamento { get; set; }
    public string? Responsavel { get; set; }
    public string? Descricao { get; set; }
    public string? MotivoPerda { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }
}

public class OrdemServico : BaseEntity, IAuditableEntity
{
    public string Numero { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public Mailing? Cliente { get; set; }
    public string DescricaoServico { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = "Aberta"; // Aberta, Em Execução, Concluída, Cancelada
    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    public DateTime? DataPrevisao { get; set; }
    public DateTime? DataConclusao { get; set; }
    public string? TecnicoResponsavel { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }
}

public class PedidoVenda : BaseEntity, IAuditableEntity
{
    public string Numero { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public Mailing? Cliente { get; set; }
    public int? VendedorId { get; set; }
    public Mailing? Vendedor { get; set; }
    public int? TransportadoraId { get; set; }
    public Mailing? Transportadora { get; set; }
    
    public StatusPedido Status { get; set; } = StatusPedido.Orcamento;
    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
    public DateTime? DataPrevisaoEntrega { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Desconto { get; set; }
    public decimal ValorFrete { get; set; }
    public decimal ValorTotal { get; set; }
    public string? CondicaoPagamento { get; set; }
    public string? Observacao { get; set; }

    public ICollection<PedidoVendaItem> Itens { get; set; } = new List<PedidoVendaItem>();

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }
}

public class PedidoVendaItem : BaseEntity
{
    public int PedidoVendaId { get; set; }
    public PedidoVenda PedidoVenda { get; set; } = null!;
    public int ProdutoId { get; set; }
    public string ProdutoCodigo { get; set; } = string.Empty;
    public string ProdutoDescricao { get; set; } = string.Empty;
    public string Unidade { get; set; } = "UN";
    public decimal Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal DescontoPct { get; set; }
    public decimal ValorTotal { get; set; }
}
