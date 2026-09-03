namespace ERP.Domain.Enums;

public enum TipoPessoa
{
    Juridica = 1, // 'J'
    Fisica = 2    // 'F'
}

public enum IndIe
{
    ContribuinteICMS = 1,
    ContribuinteIsento = 2,
    NaoContribuinte = 9
}

public enum RegimeTributario
{
    SimplesNacional = 1, // SN
    LucroPresumido = 2,  // LP
    LucroReal = 3        // LR
}

public enum PotencialMailing
{
    Alto = 1,
    Medio = 2,
    Baixo = 3
}

public enum TipoEndereco
{
    Faturamento = 1,
    Cobranca = 2,
    RetiradaEntrega = 3,
    Correspondencia = 4
}

public enum TipoCnae
{
    Principal = 1,
    Secundario = 2
}

public enum ModuloSistema
{
    Comercial,
    CRM,
    Compras,
    Contratos,
    Estoque,
    Faturamento,
    Financeiro,
    Producao,
    Sistema
}

public enum NivelAcesso
{
    SomenteLeitura = 1,
    CRUD = 2,
    Full = 3
}

public enum StatusCrm
{
    Prospeccao = 1,
    Proposta = 2,
    Negociacao = 3,
    Fechado = 4
}

public enum StatusPedido
{
    Orcamento = 1,
    PendenteAprovacao = 2,
    Aprovado = 3,
    Faturado = 4,
    Cancelado = 5
}

public enum TipoTitulo
{
    Pagar = 1,
    Receber = 2
}

public enum StatusTitulo
{
    Pendente = 1,
    Liquidado = 2,
    Parcial = 3,
    Cancelado = 4
}

public enum TipoMovimentacaoEstoque
{
    Entrada = 1,
    Saida = 2,
    Transferencia = 3,
    Ajuste = 4
}
