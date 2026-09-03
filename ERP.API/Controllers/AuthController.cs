using ERP.Application.DTOs.Auth;
using ERP.Application.Features.Auth;
using ERP.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AuthController(IMediator mediator, IAppDbContext context, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _context = context;
        _currentUser = currentUser;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var response = await _mediator.Send(new LoginCommand(dto));
        if (response == null)
        {
            return Unauthorized(new { message = "Email ou senha inválidos, ou usuário inativo." });
        }
        return Ok(response);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        var response = await _mediator.Send(new RefreshTokenCommand(dto));
        if (response == null)
        {
            return Unauthorized(new { message = "Token de atualização inválido ou expirado." });
        }
        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        if (_currentUser.UsuarioId.HasValue)
        {
            var tokens = await _context.RefreshTokens
                .Where(r => r.UsuarioId == _currentUser.UsuarioId.Value && !r.Revogado)
                .ToListAsync();

            foreach (var t in tokens)
            {
                t.Revogado = true;
            }
            await _context.SaveChangesAsync();
        }
        return Ok(new { message = "Logout efetuado com sucesso." });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        if (!_currentUser.UsuarioId.HasValue) return Unauthorized();

        var user = await _context.Usuarios
            .Include(u => u.Perfil)
                .ThenInclude(p => p.Modulos)
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UsuarioId.Value);

        if (user == null) return NotFound();

        var dto = new UsuarioDto
        {
            Id = user.Id,
            Nome = user.Nome,
            Email = user.Email,
            Ativo = user.Ativo,
            VendedorId = user.VendedorId,
            PerfilId = user.PerfilId,
            PerfilNome = user.Perfil?.Nome ?? "Usuário",
            Modulos = user.Perfil?.Modulos.Select(m => new PerfilModuloDto
            {
                Id = m.Id,
                Modulo = m.Modulo,
                NivelAcesso = m.NivelAcesso,
                PodeLer = m.PodeLer,
                PodeCriar = m.PodeCriar,
                PodeEditar = m.PodeEditar,
                PodeExcluir = m.PodeExcluir
            }).ToList() ?? new List<PerfilModuloDto>()
        };

        return Ok(dto);
    }
}
