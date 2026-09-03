using ERP.Domain.Entities.Common;
using ERP.Domain.Enums;

namespace ERP.Domain.Entities.Auth;

public class Usuario : BaseEntity, IAuditableEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public int? VendedorId { get; set; } // Vínculo opcional com Mailing vendedor
    public int PerfilId { get; set; }
    public Perfil Perfil { get; set; } = null!;
    
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

public class Perfil : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public ICollection<PerfilModulo> Modulos { get; set; } = new List<PerfilModulo>();
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}

public class PerfilModulo : BaseEntity
{
    public int PerfilId { get; set; }
    public Perfil Perfil { get; set; } = null!;
    public ModuloSistema Modulo { get; set; }
    public NivelAcesso NivelAcesso { get; set; } = NivelAcesso.CRUD;
    public bool PodeLer { get; set; } = true;
    public bool PodeCriar { get; set; } = true;
    public bool PodeEditar { get; set; } = true;
    public bool PodeExcluir { get; set; } = true;
}

public class PerfilPermissao : BaseEntity
{
    public int PerfilId { get; set; }
    public Perfil Perfil { get; set; } = null!;
    public string ChavePermissao { get; set; } = string.Empty; // ex: "Mailing.Exportar", "Desconto.Acima10Pct"
    public bool Permitido { get; set; } = true;
}

public class RefreshToken : BaseEntity
{
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public DateTime DataExpiracao { get; set; }
    public bool Revogado { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
