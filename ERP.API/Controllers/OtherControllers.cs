using ERP.Application.DTOs.Comercial;
using ERP.Application.DTOs.Estoque;
using ERP.Application.DTOs.Faturamento;
using ERP.Application.DTOs.Financeiro;
using ERP.Application.DTOs.Sistema;
using ERP.Application.Interfaces;
using ERP.Domain.Entities.Comercial;
using ERP.Domain.Entities.Estoque;
using ERP.Domain.Entities.Financeiro;
using ERP.Domain.Entities.Sistema;
using ERP.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComercialController : ControllerBase
{
    private readonly IAppDbContext _context;

    public ComercialController(IAppDbContext context)
    {
        _context = context;
    }

    [HttpGet("crm/oportunidades")]
    public async Task<IActionResult> GetOportunidades()
    {
        var items = await _context.OportunidadesCrm
            .Include(o => o.Mailing)
            .OrderByDescending(o => o.Id)
            .Select(o => new OportunidadeCrmDto
            {
                Id = o.Id,
                MailingId = o.MailingId,
                ClienteNome = o.Mailing != null ? (o.Mailing.RazaoSocial ?? o.Mailing.NomeCompleto ?? "") : "Sem Cliente",
                Titulo = o.Titulo,
                ValorEstimado = o.ValorEstimado,
                Status = o.Status,
                ProbabilidadePct = o.ProbabilidadePct,
                PrevisaoFechamento = o.PrevisaoFechamento,
                Responsavel = o.Responsavel,
                Descricao = o.Descricao
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost("crm/oportunidades")]
    public async Task<IActionResult> CreateOportunidade([FromBody] OportunidadeCrmDto dto)
    {
        var op = new OportunidadeCrm
        {
            MailingId = dto.MailingId,
            Titulo = dto.Titulo,
            ValorEstimado = dto.ValorEstimado,
            Status = dto.Status,
            ProbabilidadePct = dto.ProbabilidadePct,
            PrevisaoFechamento = dto.PrevisaoFechamento,
            Responsavel = dto.Responsavel ?? User.Identity?.Name ?? "Vendedor",
            Descricao = dto.Descricao,
            CriadoEm = DateTime.UtcNow
        };
        _context.OportunidadesCrm.Add(op);
        await _context.SaveChangesAsync();
        return Ok(new { id = op.Id, message = "Oportunidade criada com sucesso." });
    }

    [HttpPatch("crm/oportunidades/{id:int}/status")]
    public async Task<IActionResult> UpdateOportunidadeStatus(int id, [FromBody] StatusCrm novoStatus)
    {
        var op = await _context.OportunidadesCrm.FindAsync(id);
        if (op == null) return NotFound();
        op.Status = novoStatus;
        op.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Status atualizado com sucesso." });
    }

    [HttpGet("pedidos")]
    public async Task<IActionResult> GetPedidos()
    {
        var items = await _context.PedidosVenda
            .Include(p => p.Cliente)
            .Include(p => p.Vendedor)
            .Include(p => p.Itens)
            .OrderByDescending(p => p.Id)
            .Select(p => new PedidoVendaDto
            {
                Id = p.Id,
                Numero = p.Numero,
                ClienteId = p.ClienteId,
                ClienteNome = p.Cliente != null ? (p.Cliente.RazaoSocial ?? p.Cliente.NomeCompleto ?? "") : "",
                VendedorNome = p.Vendedor != null ? (p.Vendedor.RazaoSocial ?? p.Vendedor.NomeCompleto) : null,
                Status = p.Status,
                DataEmissao = p.DataEmissao,
                DataPrevisaoEntrega = p.DataPrevisaoEntrega,
                SubTotal = p.SubTotal,
                Desconto = p.Desconto,
                ValorFrete = p.ValorFrete,
                ValorTotal = p.ValorTotal,
                Itens = p.Itens.Select(i => new PedidoVendaItemDto
                {
                    Id = i.Id,
                    ProdutoId = i.ProdutoId,
                    ProdutoCodigo = i.ProdutoCodigo,
                    ProdutoDescricao = i.ProdutoDescricao,
                    Unidade = i.Unidade,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    DescontoPct = i.DescontoPct,
                    ValorTotal = i.ValorTotal
                }).ToList()
            })
            .ToListAsync();

        return Ok(items);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EstoqueController : ControllerBase
{
    private readonly IAppDbContext _context;

    public EstoqueController(IAppDbContext context)
    {
        _context = context;
    }

    [HttpGet("produtos")]
    public async Task<IActionResult> GetProdutos()
    {
        var items = await _context.Produtos
            .OrderBy(p => p.Descricao)
            .Select(p => new ProdutoDto
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Descricao = p.Descricao,
                Unidade = p.Unidade,
                Ncm = p.Ncm,
                Grupo = p.Grupo,
                PrecoCusto = p.PrecoCusto,
                PrecoVenda = p.PrecoVenda,
                EstoqueAtual = p.EstoqueAtual,
                EstoqueMinimo = p.EstoqueMinimo,
                Ativo = p.Ativo
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost("produtos")]
    public async Task<IActionResult> CreateProduto([FromBody] ProdutoDto dto)
    {
        var prod = new Produto
        {
            Codigo = dto.Codigo,
            Descricao = dto.Descricao,
            Unidade = dto.Unidade,
            Ncm = dto.Ncm,
            Grupo = dto.Grupo,
            PrecoCusto = dto.PrecoCusto,
            PrecoVenda = dto.PrecoVenda,
            MargemLucroPct = dto.PrecoCusto > 0 ? ((dto.PrecoVenda - dto.PrecoCusto) / dto.PrecoCusto) * 100 : 0,
            EstoqueAtual = dto.EstoqueAtual,
            EstoqueMinimo = dto.EstoqueMinimo,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };
        _context.Produtos.Add(prod);
        await _context.SaveChangesAsync();
        return Ok(new { id = prod.Id, message = "Produto cadastrado com sucesso." });
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinanceiroController : ControllerBase
{
    private readonly IAppDbContext _context;

    public FinanceiroController(IAppDbContext context)
    {
        _context = context;
    }

    [HttpGet("titulos")]
    public async Task<IActionResult> GetTitulos([FromQuery] TipoTitulo? tipo)
    {
        var query = _context.TitulosFinanceiros
            .Include(t => t.Mailing)
            .AsQueryable();

        if (tipo.HasValue)
        {
            query = query.Where(t => t.Tipo == tipo.Value);
        }

        var items = await query
            .OrderBy(t => t.DataVencimento)
            .Select(t => new TituloFinanceiroDto
            {
                Id = t.Id,
                Tipo = t.Tipo,
                NumeroDocumento = t.NumeroDocumento,
                MailingId = t.MailingId,
                SacadoCedenteNome = t.SacadoCedenteNome ?? (t.Mailing != null ? (t.Mailing.RazaoSocial ?? t.Mailing.NomeCompleto) : ""),
                DataEmissao = t.DataEmissao,
                DataVencimento = t.DataVencimento,
                DataLiquidacao = t.DataLiquidacao,
                ValorOriginal = t.ValorOriginal,
                ValorPagoLiquidado = t.ValorPagoLiquidado,
                ValorSaldo = t.ValorSaldo,
                Status = t.Status,
                FormaPagamento = t.FormaPagamento,
                Historico = t.Historico
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("fluxo-caixa")]
    public async Task<IActionResult> GetFluxoCaixa()
    {
        var saldoBancos = await _context.ContasBancarias.SumAsync(c => c.SaldoAtual);
        var hoje = DateTime.UtcNow.Date;
        var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
        var fimMes = inicioMes.AddMonths(1).AddDays(-1);

        var receberHoje = await _context.TitulosFinanceiros
            .Where(t => t.Tipo == TipoTitulo.Receber && t.Status == StatusTitulo.Pendente && t.DataVencimento.Date == hoje)
            .SumAsync(t => t.ValorSaldo);

        var pagarHoje = await _context.TitulosFinanceiros
            .Where(t => t.Tipo == TipoTitulo.Pagar && t.Status == StatusTitulo.Pendente && t.DataVencimento.Date == hoje)
            .SumAsync(t => t.ValorSaldo);

        var receberMes = await _context.TitulosFinanceiros
            .Where(t => t.Tipo == TipoTitulo.Receber && t.Status == StatusTitulo.Pendente && t.DataVencimento.Date >= inicioMes && t.DataVencimento.Date <= fimMes)
            .SumAsync(t => t.ValorSaldo);

        var pagarMes = await _context.TitulosFinanceiros
            .Where(t => t.Tipo == TipoTitulo.Pagar && t.Status == StatusTitulo.Pendente && t.DataVencimento.Date >= inicioMes && t.DataVencimento.Date <= fimMes)
            .SumAsync(t => t.ValorSaldo);

        return Ok(new FluxoCaixaDto
        {
            SaldoBancos = saldoBancos,
            TotalReceberHoje = receberHoje,
            TotalPagarHoje = pagarHoje,
            TotalReceberMes = receberMes,
            TotalPagarMes = pagarMes
        });
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SistemaController : ControllerBase
{
    private readonly IAppDbContext _context;

    public SistemaController(IAppDbContext context)
    {
        _context = context;
    }

    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs()
    {
        var logs = await _context.AuditLogs
            .OrderByDescending(l => l.DataHora)
            .Take(100)
            .Select(l => new AuditLogDto
            {
                Id = l.Id,
                DataHora = l.DataHora,
                UsuarioNome = l.UsuarioNome,
                Modulo = l.Modulo,
                Entidade = l.Entidade,
                Operacao = l.Operacao,
                RegistroId = l.RegistroId,
                Detalhes = l.Detalhes
            })
            .ToListAsync();

        return Ok(logs);
    }

    [HttpGet("empresa")]
    public async Task<IActionResult> GetEmpresa()
    {
        var emp = await _context.Empresas.FirstOrDefaultAsync();
        if (emp == null) return NotFound();

        return Ok(new EmpresaDto
        {
            Id = emp.Id,
            RazaoSocial = emp.RazaoSocial,
            NomeFantasia = emp.NomeFantasia,
            Cnpj = emp.Cnpj,
            Ie = emp.Ie,
            Endereco = emp.Endereco,
            Cidade = emp.Cidade,
            Estado = emp.Estado,
            Telefone = emp.Telefone,
            SefazAmbiente = emp.SefazAmbiente
        });
    }
}
