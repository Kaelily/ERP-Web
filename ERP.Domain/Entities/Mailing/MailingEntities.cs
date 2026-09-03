using ERP.Domain.Entities.Common;
using ERP.Domain.Enums;

namespace ERP.Domain.Entities.Mailings;

public class Mailing : BaseEntity, IAuditableEntity
{
    public bool Inativo { get; set; } = false;
    public bool IsCliente { get; set; } = true;
    public bool IsFornecedor { get; set; } = false;
    public bool IsTransportadora { get; set; } = false;
    public bool IsIntermediador { get; set; } = false;
    public bool IsFuncionario { get; set; } = false;

    public int? VendedorId { get; set; }
    public Mailing? Vendedor { get; set; }

    public TipoPessoa TipoPessoa { get; set; } = TipoPessoa.Juridica;
    public string? RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public string? NomeCompleto { get; set; }
    public string? Cnpj { get; set; }
    public string? Cpf { get; set; }
    public string? Ie { get; set; }
    public string? Im { get; set; }
    public string? Rne { get; set; }
    public string? Rg { get; set; }
    public IndIe IndIe { get; set; } = IndIe.ContribuinteICMS;
    public string? TipoConsumidor { get; set; }
    public RegimeTributario? RegimeTributario { get; set; }
    public string? Alertas { get; set; }
    public string? Observacao { get; set; }
    public string? Ranqueamento { get; set; }
    public PotencialMailing? Potencial { get; set; } = PotencialMailing.Medio;
    public string? Origem { get; set; }
    public decimal? ToleranciaProducao { get; set; } // % para alumínio

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }

    // Child Collections (1:N)
    public ICollection<MailingEndereco> Enderecos { get; set; } = new List<MailingEndereco>();
    public ICollection<MailingContato> Contatos { get; set; } = new List<MailingContato>();
    public ICollection<MailingPreferencia> Preferencias { get; set; } = new List<MailingPreferencia>();
    public ICollection<MailingCnae> Cnaes { get; set; } = new List<MailingCnae>();
    public ICollection<MailingAcao> Acoes { get; set; } = new List<MailingAcao>();
    public ICollection<MailingFollowUp> FollowUps { get; set; } = new List<MailingFollowUp>();
    public ICollection<MailingDadoBancario> DadosBancarios { get; set; } = new List<MailingDadoBancario>();
    public ICollection<MailingVeiculo> Veiculos { get; set; } = new List<MailingVeiculo>();
    public ICollection<MailingRegiao> Regioes { get; set; } = new List<MailingRegiao>();
    public ICollection<MailingDocumento> Documentos { get; set; } = new List<MailingDocumento>();
    public MailingFaturamento? Faturamento { get; set; }
}

public class MailingEndereco : BaseEntity
{
    public int MailingId { get; set; }
    public Mailing Mailing { get; set; } = null!;
    public TipoEndereco TipoEnd { get; set; } = TipoEndereco.Faturamento;
    public string Cep { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Pais { get; set; } = "Brasil";
    public bool Principal { get; set; } = false;
}

public class MailingContato : BaseEntity
{
    public int MailingId { get; set; }
    public Mailing Mailing { get; set; } = null!;
    public string Nome { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public string? TelComercial { get; set; }
    public string? Celular { get; set; }
    public string? Email { get; set; }
    
    // Setores de contato
    public bool CP { get; set; } // Compras
    public bool VE { get; set; } // Vendas
    public bool FI { get; set; } // Financeiro
    public bool FAT { get; set; } // Faturamento
}

public class MailingPreferencia : BaseEntity
{
    public int MailingId { get; set; }
    public Mailing Mailing { get; set; } = null!;
    public int GrupoId { get; set; }
    public string GrupoNome { get; set; } = string.Empty;
    public int? SubGrupoId { get; set; }
    public string? SubGrupoNome { get; set; }
}

public class MailingCnae : BaseEntity
{
    public int MailingId { get; set; }
    public Mailing Mailing { get; set; } = null!;
    public TipoCnae Tipo { get; set; } = TipoCnae.Secundario;
    public string CnaeCodigo { get; set; } = string.Empty;
    public string CnaeDescricao { get; set; } = string.Empty;
}

public class MailingAcao : BaseEntity
{
    public int MailingId { get; set; }
    public Mailing Mailing { get; set; } = null!;
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public string TipoAcao { get; set; } = string.Empty; // Telefonema, Reunião, Email, Visita
    public string Acao { get; set; } = string.Empty;
    public string? Resultado { get; set; }
    public string? Justificativa { get; set; }
    public string? UsuarioNome { get; set; }
}

public class MailingFollowUp : BaseEntity
{
    public int MailingId { get; set; }
    public Mailing Mailing { get; set; } = null!;
    public DateTime DataRetorno { get; set; }
    public int? UsuarioId { get; set; }
    public string? UsuarioNome { get; set; }
    public string Assunto { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Encerrado { get; set; } = false;
    public DateTime? DataEncerramento { get; set; }
}

public class MailingDadoBancario : BaseEntity
{
    public int MailingId { get; set; }
    public Mailing Mailing { get; set; } = null!;
    public string Status { get; set; } = "Ativo";
    public string Tipo { get; set; } = "Corrente"; // Corrente, Poupança, Pagamento
    public string BancoCodigo { get; set; } = string.Empty;
    public string BancoNome { get; set; } = string.Empty;
    public string Agencia { get; set; } = string.Empty;
    public string? DigitoAgencia { get; set; }
    public string Conta { get; set; } = string.Empty;
    public string? DigitoConta { get; set; }
    public string? Favorecido { get; set; }
    public string? CnpjCpf { get; set; }
    public string? ChavePix { get; set; }
}

public class MailingVeiculo : BaseEntity
{
    public int MailingId { get; set; }
    public Mailing Mailing { get; set; } = null!;
    public string TipoVeiculo { get; set; } = "Caminhão"; // Carreta, Toco, Van, etc.
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public string? Antt { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public decimal? TaraKg { get; set; }
    public decimal? CapacidadeKg { get; set; }
}

public class MailingRegiao : BaseEntity
{
    public int MailingId { get; set; }
    public Mailing Mailing { get; set; } = null!;
    public string Estado { get; set; } = string.Empty;
    public string? CidadeNome { get; set; }
    public int PrazoDias { get; set; } = 1;
    public decimal? ValorFreteKg { get; set; }
}

public class MailingDocumento : BaseEntity
{
    public int MailingId { get; set; }
    public Mailing Mailing { get; set; } = null!;
    public string Descricao { get; set; } = string.Empty;
    public int? UsuarioId { get; set; }
    public string? UsuarioNome { get; set; }
    public DateTime DataHora { get; set; } = DateTime.UtcNow;
    public string NomeArquivo { get; set; } = string.Empty;
    public string? TipoConteudo { get; set; }
    public long TamanhoBytes { get; set; }
    public string StoragePath { get; set; } = string.Empty;
}

public class MailingFaturamento : BaseEntity
{
    public int MailingId { get; set; }
    public Mailing Mailing { get; set; } = null!;
    public int? ListaPrecoId { get; set; }
    public string? ListaPrecoNome { get; set; }
    public int? FormaPagtoId { get; set; }
    public string? FormaPagtoNome { get; set; }
    public int? CentroCustoId { get; set; }
    public string? CentroCustoNome { get; set; }
    public decimal ComissaoPct { get; set; } = 0;
    public int? TransportadoraId { get; set; }
    public string? TransportadoraNome { get; set; }
    public decimal ValorFrete { get; set; } = 0;
    public decimal LimiteCredito { get; set; } = 0;
    public bool Bloqueado { get; set; } = false;
    public string? MotivoBloqueio { get; set; }
    public int? DiaPagamento { get; set; }
    public bool UsarOutroCadastroNF { get; set; } = false;
    public int? MailingNFId { get; set; }
}
