using ERP.Domain.Enums;

namespace ERP.Application.DTOs.Mailing;

public class MailingListItemDto
{
    public int Id { get; set; }
    public bool Inativo { get; set; }
    public bool IsCliente { get; set; }
    public bool IsFornecedor { get; set; }
    public bool IsTransportadora { get; set; }
    public bool IsIntermediador { get; set; }
    public bool IsFuncionario { get; set; }
    public TipoPessoa TipoPessoa { get; set; }
    public string Nome { get; set; } = string.Empty; // RazaoSocial ou NomeCompleto
    public string? NomeFantasia { get; set; }
    public string? Documento { get; set; } // CNPJ ou CPF formatado
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? VendedorNome { get; set; }
    public PotencialMailing? Potencial { get; set; }
    public int? DiasSemCompra { get; set; }
    public bool Bloqueado { get; set; }
}

public class MailingFilterDto
{
    public string? Termo { get; set; }
    public string? Tipo { get; set; } // Cliente, Fornecedor, Transportadora, Funcionario, Todos
    public int? VendedorId { get; set; }
    public bool? Inativo { get; set; }
    public bool? Bloqueado { get; set; }
    public int? DiasSemCompraMinimo { get; set; }
    public int? GrupoPreferenciaId { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}

public class MailingDto
{
    public int Id { get; set; }
    public bool Inativo { get; set; }
    public bool IsCliente { get; set; }
    public bool IsFornecedor { get; set; }
    public bool IsTransportadora { get; set; }
    public bool IsIntermediador { get; set; }
    public bool IsFuncionario { get; set; }
    public int? VendedorId { get; set; }
    public string? VendedorNome { get; set; }
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
    public PotencialMailing? Potencial { get; set; }
    public string? Origem { get; set; }
    public decimal? ToleranciaProducao { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }

    // 10 Nested child objects/lists
    public List<MailingEnderecoDto> Enderecos { get; set; } = new();
    public List<MailingContatoDto> Contatos { get; set; } = new();
    public List<MailingPreferenciaDto> Preferencias { get; set; } = new();
    public List<MailingCnaeDto> Cnaes { get; set; } = new();
    public List<MailingAcaoDto> Acoes { get; set; } = new();
    public List<MailingFollowUpDto> FollowUps { get; set; } = new();
    public List<MailingDadoBancarioDto> DadosBancarios { get; set; } = new();
    public List<MailingVeiculoDto> Veiculos { get; set; } = new();
    public List<MailingRegiaoDto> Regioes { get; set; } = new();
    public List<MailingDocumentoDto> Documentos { get; set; } = new();
    public MailingFaturamentoDto? Faturamento { get; set; }
}

public class MailingCreateDto
{
    public bool Inativo { get; set; }
    public bool IsCliente { get; set; } = true;
    public bool IsFornecedor { get; set; }
    public bool IsTransportadora { get; set; }
    public bool IsIntermediador { get; set; }
    public bool IsFuncionario { get; set; }
    public int? VendedorId { get; set; }
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
    public PotencialMailing? Potencial { get; set; }
    public string? Origem { get; set; }
    public decimal? ToleranciaProducao { get; set; }

    public List<MailingEnderecoDto> Enderecos { get; set; } = new();
    public List<MailingContatoDto> Contatos { get; set; } = new();
    public List<MailingPreferenciaDto> Preferencias { get; set; } = new();
    public List<MailingCnaeDto> Cnaes { get; set; } = new();
    public List<MailingAcaoDto> Acoes { get; set; } = new();
    public List<MailingFollowUpDto> FollowUps { get; set; } = new();
    public List<MailingDadoBancarioDto> DadosBancarios { get; set; } = new();
    public List<MailingVeiculoDto> Veiculos { get; set; } = new();
    public List<MailingRegiaoDto> Regioes { get; set; } = new();
    public List<MailingDocumentoDto> Documentos { get; set; } = new();
    public MailingFaturamentoDto? Faturamento { get; set; }
}

public class MailingUpdateDto : MailingCreateDto
{
    public int Id { get; set; }
}

public class MailingEnderecoDto
{
    public int Id { get; set; }
    public int MailingId { get; set; }
    public TipoEndereco TipoEnd { get; set; } = TipoEndereco.Faturamento;
    public string Cep { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Pais { get; set; } = "Brasil";
    public bool Principal { get; set; }
}

public class MailingContatoDto
{
    public int Id { get; set; }
    public int MailingId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public string? TelComercial { get; set; }
    public string? Celular { get; set; }
    public string? Email { get; set; }
    public bool CP { get; set; }
    public bool VE { get; set; }
    public bool FI { get; set; }
    public bool FAT { get; set; }
}

public class MailingPreferenciaDto
{
    public int Id { get; set; }
    public int MailingId { get; set; }
    public int GrupoId { get; set; }
    public string GrupoNome { get; set; } = string.Empty;
    public int? SubGrupoId { get; set; }
    public string? SubGrupoNome { get; set; }
}

public class MailingCnaeDto
{
    public int Id { get; set; }
    public int MailingId { get; set; }
    public TipoCnae Tipo { get; set; } = TipoCnae.Secundario;
    public string CnaeCodigo { get; set; } = string.Empty;
    public string CnaeDescricao { get; set; } = string.Empty;
}

public class MailingAcaoDto
{
    public int Id { get; set; }
    public int MailingId { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public string TipoAcao { get; set; } = "Contato";
    public string Acao { get; set; } = string.Empty;
    public string? Resultado { get; set; }
    public string? Justificativa { get; set; }
    public string? UsuarioNome { get; set; }
}

public class MailingFollowUpDto
{
    public int Id { get; set; }
    public int MailingId { get; set; }
    public DateTime DataRetorno { get; set; } = DateTime.UtcNow.AddDays(3);
    public int? UsuarioId { get; set; }
    public string? UsuarioNome { get; set; }
    public string Assunto { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Encerrado { get; set; }
    public DateTime? DataEncerramento { get; set; }
}

public class MailingDadoBancarioDto
{
    public int Id { get; set; }
    public int MailingId { get; set; }
    public string Status { get; set; } = "Ativo";
    public string Tipo { get; set; } = "Corrente";
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

public class MailingVeiculoDto
{
    public int Id { get; set; }
    public int MailingId { get; set; }
    public string TipoVeiculo { get; set; } = "Caminhão";
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public string? Antt { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public decimal? TaraKg { get; set; }
    public decimal? CapacidadeKg { get; set; }
}

public class MailingRegiaoDto
{
    public int Id { get; set; }
    public int MailingId { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? CidadeNome { get; set; }
    public int PrazoDias { get; set; } = 1;
    public decimal? ValorFreteKg { get; set; }
}

public class MailingDocumentoDto
{
    public int Id { get; set; }
    public int MailingId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int? UsuarioId { get; set; }
    public string? UsuarioNome { get; set; }
    public DateTime DataHora { get; set; } = DateTime.UtcNow;
    public string NomeArquivo { get; set; } = string.Empty;
    public string? TipoConteudo { get; set; }
    public long TamanhoBytes { get; set; }
    public string StoragePath { get; set; } = string.Empty;
}

public class MailingFaturamentoDto
{
    public int Id { get; set; }
    public int MailingId { get; set; }
    public int? ListaPrecoId { get; set; }
    public string? ListaPrecoNome { get; set; }
    public int? FormaPagtoId { get; set; }
    public string? FormaPagtoNome { get; set; }
    public int? CentroCustoId { get; set; }
    public string? CentroCustoNome { get; set; }
    public decimal ComissaoPct { get; set; }
    public int? TransportadoraId { get; set; }
    public string? TransportadoraNome { get; set; }
    public decimal ValorFrete { get; set; }
    public decimal LimiteCredito { get; set; }
    public bool Bloqueado { get; set; }
    public string? MotivoBloqueio { get; set; }
    public int? DiaPagamento { get; set; }
    public bool UsarOutroCadastroNF { get; set; }
    public int? MailingNFId { get; set; }
}

public class MailingEstatisticasDto
{
    public int MailingId { get; set; }
    public decimal TotalFaturadoHistorico { get; set; }
    public decimal TotalFaturadoAnoAtual { get; set; }
    public decimal TicketMedio { get; set; }
    public int QuantidadePedidos { get; set; }
    public DateTime? DataUltimaCompra { get; set; }
    public int DiasSemComprar { get; set; }
    public decimal SaldoTitulosAberto { get; set; }
    public decimal SaldoTitulosVencidos { get; set; }
    public int QuantidadeOrdensServico { get; set; }
    public List<EstatisticaMensalDto> FaturamentoMensal { get; set; } = new();
}

public class EstatisticaMensalDto
{
    public string MesAno { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
