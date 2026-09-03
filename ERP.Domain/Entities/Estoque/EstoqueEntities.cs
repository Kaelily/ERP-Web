using ERP.Domain.Entities.Common;
using ERP.Domain.Enums;

namespace ERP.Domain.Entities.Estoque;

public class Produto : BaseEntity, IAuditableEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Unidade { get; set; } = "UN";
    public string? Ncm { get; set; }
    public string? Cest { get; set; }
    public string? Grupo { get; set; }
    public decimal PrecoCusto { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal MargemLucroPct { get; set; }
    public decimal EstoqueAtual { get; set; }
    public decimal EstoqueMinimo { get; set; }
    public decimal EstoqueMaximo { get; set; }
    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }
}

public class LocalEstoque : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty; // ex: Galpão Principal, Almoxarifado A, Loja 1
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;
}

public class Lote : BaseEntity
{
    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }
    public string NumeroLote { get; set; } = string.Empty;
    public DateTime DataFabricacao { get; set; }
    public DateTime? DataValidade { get; set; }
    public decimal QuantidadeOriginal { get; set; }
    public decimal SaldoAtual { get; set; }
    public bool Ativo { get; set; } = true;
}

public class MovimentacaoEstoque : BaseEntity
{
    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }
    public int? LocalEstoqueId { get; set; }
    public LocalEstoque? LocalEstoque { get; set; }
    public TipoMovimentacaoEstoque Tipo { get; set; } = TipoMovimentacaoEstoque.Entrada;
    public decimal Quantidade { get; set; }
    public decimal CustoUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public string? DocumentoReferencia { get; set; } // ex: NF 1234, Pedido 456
    public string? Motivo { get; set; }
    public DateTime DataMovimento { get; set; } = DateTime.UtcNow;
    public string? UsuarioNome { get; set; }
}
