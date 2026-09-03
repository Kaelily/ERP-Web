using ERP.Domain.Enums;

namespace ERP.Application.DTOs.Auth;

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public UsuarioDto Usuario { get; set; } = null!;
}

public class RefreshTokenRequestDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public int? VendedorId { get; set; }
    public int PerfilId { get; set; }
    public string PerfilNome { get; set; } = string.Empty;
    public List<PerfilModuloDto> Modulos { get; set; } = new();
    public List<string> Permissoes { get; set; } = new();
}

public class PerfilDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public List<PerfilModuloDto> Modulos { get; set; } = new();
}

public class PerfilModuloDto
{
    public int Id { get; set; }
    public ModuloSistema Modulo { get; set; }
    public NivelAcesso NivelAcesso { get; set; }
    public bool PodeLer { get; set; }
    public bool PodeCriar { get; set; }
    public bool PodeEditar { get; set; }
    public bool PodeExcluir { get; set; }
}
