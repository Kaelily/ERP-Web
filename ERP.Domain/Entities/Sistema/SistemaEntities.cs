using ERP.Domain.Entities.Common;

namespace ERP.Domain.Entities.Sistema;

public class AuditLog : BaseEntity
{
    public DateTime DataHora { get; set; } = DateTime.UtcNow;
    public string? UsuarioNome { get; set; }
    public string Modulo { get; set; } = string.Empty;
    public string Entidade { get; set; } = string.Empty;
    public string Operacao { get; set; } = string.Empty; // Create, Update, Delete, Inactivate
    public string? RegistroId { get; set; }
    public string? Detalhes { get; set; }
    public string? IpOrigem { get; set; }
}

public class Empresa : BaseEntity
{
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Ie { get; set; } = string.Empty;
    public string? Im { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? CertificadoDigitalNome { get; set; }
    public DateTime? CertificadoValidade { get; set; }
    public string SefazAmbiente { get; set; } = "Homologação"; // Produção, Homologação
}

public class ParametrosSistema : BaseEntity
{
    public string Chave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Categoria { get; set; } = "Geral";
}

public class Feriado : BaseEntity
{
    public DateTime Data { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool FixoAnual { get; set; } = true;
}
