using ERP.API.Hubs;
using ERP.Application.DTOs.Mailing;
using ERP.Application.Features.Mailing.Commands;
using ERP.Application.Features.Mailing.Queries;
using ERP.Application.Interfaces;
using ERP.Domain.Entities.Mailings;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MailingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAppDbContext _context;
    private readonly IExternalLookupService _lookupService;
    private readonly IValidator<MailingCreateDto> _createValidator;
    private readonly IValidator<MailingUpdateDto> _updateValidator;
    private readonly IHubContext<ErpNotificationHub> _hubContext;

    public MailingController(
        IMediator mediator,
        IAppDbContext context,
        IExternalLookupService lookupService,
        IValidator<MailingCreateDto> createValidator,
        IValidator<MailingUpdateDto> updateValidator,
        IHubContext<ErpNotificationHub> hubContext)
    {
        _mediator = mediator;
        _context = context;
        _lookupService = lookupService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] MailingFilterDto filter)
    {
        var result = await _mediator.Send(new GetMailingListQuery(filter));
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetMailingByIdQuery(id));
        if (result == null) return NotFound(new { message = $"Mailing #{id} não encontrado." });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MailingCreateDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Verifica duplicidade de CNPJ/CPF se informado
        if (!string.IsNullOrWhiteSpace(dto.Cnpj))
        {
            var cleanCnpj = dto.Cnpj.Trim();
            var exists = await _context.Mailings.AnyAsync(m => m.Cnpj == cleanCnpj);
            if (exists)
            {
                return Conflict(new { message = $"Já existe um cadastro com o CNPJ {dto.Cnpj}." });
            }
        }
        else if (!string.IsNullOrWhiteSpace(dto.Cpf))
        {
            var cleanCpf = dto.Cpf.Trim();
            var exists = await _context.Mailings.AnyAsync(m => m.Cpf == cleanCpf);
            if (exists)
            {
                return Conflict(new { message = $"Já existe um cadastro com o CPF {dto.Cpf}." });
            }
        }

        var id = await _mediator.Send(new CreateMailingCommand(dto));
        
        await _hubContext.Clients.All.SendAsync("MailingUpdated", id, "Created", DateTime.UtcNow);

        return CreatedAtAction(nameof(GetById), new { id }, new { id, message = "Mailing cadastrado com sucesso." });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] MailingUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest(new { message = "Id divergente." });

        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var updated = await _mediator.Send(new UpdateMailingCommand(dto));
        if (!updated) return NotFound(new { message = $"Mailing #{id} não encontrado." });

        await _hubContext.Clients.All.SendAsync("MailingUpdated", id, "Updated", DateTime.UtcNow);

        return Ok(new { message = "Mailing atualizado com sucesso." });
    }

    [HttpPatch("{id:int}/inativar")]
    public async Task<IActionResult> Inativar(int id)
    {
        var result = await _mediator.Send(new InativarMailingCommand(id));
        if (!result) return NotFound(new { message = $"Mailing #{id} não encontrado." });

        await _hubContext.Clients.All.SendAsync("MailingUpdated", id, "StatusToggled", DateTime.UtcNow);

        return Ok(new { message = "Status de inativação atualizado." });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteMailingCommand(id));
        if (!result) return NotFound(new { message = $"Mailing #{id} não encontrado." });

        await _hubContext.Clients.All.SendAsync("MailingUpdated", id, "Deleted", DateTime.UtcNow);

        return Ok(new { message = "Mailing excluído com sucesso." });
    }

    [HttpGet("{id:int}/estatisticas")]
    public async Task<IActionResult> GetEstatisticas(int id)
    {
        var result = await _mediator.Send(new GetMailingEstatisticasQuery(id));
        if (result == null) return NotFound(new { message = $"Mailing #{id} não encontrado." });
        return Ok(result);
    }

    public class UploadDocumentoRequest
    {
        public IFormFile? File { get; set; }
        public string? Descricao { get; set; }
    }

    [HttpPost("{id:int}/documentos")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadDocumento(int id, [FromForm] UploadDocumentoRequest request)
    {
        var mailing = await _context.Mailings.FindAsync(id);
        if (mailing == null) return NotFound(new { message = $"Mailing #{id} não encontrado." });

        var fileName = request.File?.FileName ?? $"Documento_{DateTime.UtcNow.Ticks}.pdf";
        var fileSize = request.File?.Length ?? 1024 * 150;
        var contentType = request.File?.ContentType ?? "application/pdf";

        var doc = new MailingDocumento
        {
            MailingId = id,
            Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? "Anexo Comercial" : request.Descricao,
            NomeArquivo = fileName,
            TipoConteudo = contentType,
            TamanhoBytes = fileSize,
            StoragePath = $"gs://erp-docs/mailing/{id}/{fileName}",
            DataHora = DateTime.UtcNow,
            UsuarioNome = User.Identity?.Name ?? "Admin"
        };

        _context.MailingDocumentos.Add(doc);
        await _context.SaveChangesAsync();

        return Ok(new { id = doc.Id, message = "Documento enviado com sucesso." });
    }

    [HttpGet("{id:int}/documentos/{docId:int}/download")]
    public async Task<IActionResult> DownloadDocumento(int id, int docId)
    {
        var doc = await _context.MailingDocumentos.FirstOrDefaultAsync(d => d.Id == docId && d.MailingId == id);
        if (doc == null) return NotFound(new { message = "Documento não encontrado." });

        var sampleContent = System.Text.Encoding.UTF8.GetBytes($"Conteúdo de demonstração do arquivo '{doc.NomeArquivo}' referente ao Mailing #{id}.");
        return File(sampleContent, doc.TipoConteudo ?? "application/octet-stream", doc.NomeArquivo);
    }

    [HttpGet("lookup/cep/{cep}")]
    [AllowAnonymous]
    public async Task<IActionResult> LookupCep(string cep)
    {
        var result = await _lookupService.BuscarCepAsync(cep);
        if (result == null) return NotFound(new { message = "CEP não encontrado." });
        return Ok(result);
    }

    [HttpGet("lookup/cnpj/{cnpj}")]
    [AllowAnonymous]
    public async Task<IActionResult> LookupCnpj(string cnpj)
    {
        var result = await _lookupService.BuscarCnpjAsync(cnpj);
        if (result == null) return NotFound(new { message = "CNPJ não encontrado." });
        return Ok(result);
    }
}
