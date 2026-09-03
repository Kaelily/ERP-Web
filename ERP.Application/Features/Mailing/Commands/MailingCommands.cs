using ERP.Application.DTOs.Mailing;
using ERP.Application.Interfaces;
using ERP.Domain.Entities.Mailings;
using ERP.Domain.Entities.Sistema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Features.Mailing.Commands;

public record CreateMailingCommand(MailingCreateDto Dto) : IRequest<int>;

public class CreateMailingHandler : IRequestHandler<CreateMailingCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateMailingHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateMailingCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var mailing = new Domain.Entities.Mailings.Mailing
        {
            Inativo = dto.Inativo,
            IsCliente = dto.IsCliente,
            IsFornecedor = dto.IsFornecedor,
            IsTransportadora = dto.IsTransportadora,
            IsIntermediador = dto.IsIntermediador,
            IsFuncionario = dto.IsFuncionario,
            VendedorId = dto.VendedorId,
            TipoPessoa = dto.TipoPessoa,
            RazaoSocial = dto.RazaoSocial,
            NomeFantasia = dto.NomeFantasia,
            NomeCompleto = dto.NomeCompleto,
            Cnpj = dto.Cnpj,
            Cpf = dto.Cpf,
            Ie = dto.Ie,
            Im = dto.Im,
            Rne = dto.Rne,
            Rg = dto.Rg,
            IndIe = dto.IndIe,
            TipoConsumidor = dto.TipoConsumidor,
            RegimeTributario = dto.RegimeTributario,
            Alertas = dto.Alertas,
            Observacao = dto.Observacao,
            Ranqueamento = dto.Ranqueamento,
            Potencial = dto.Potencial,
            Origem = dto.Origem,
            ToleranciaProducao = dto.ToleranciaProducao,
            CriadoEm = DateTime.UtcNow,
            CriadoPor = _currentUser.UsuarioNome ?? "Sistema"
        };

        if (dto.Enderecos != null && dto.Enderecos.Any())
        {
            foreach (var e in dto.Enderecos)
            {
                mailing.Enderecos.Add(new MailingEndereco
                {
                    TipoEnd = e.TipoEnd,
                    Cep = e.Cep,
                    Logradouro = e.Logradouro,
                    Numero = e.Numero,
                    Complemento = e.Complemento,
                    Bairro = e.Bairro,
                    Cidade = e.Cidade,
                    Estado = e.Estado,
                    Pais = string.IsNullOrWhiteSpace(e.Pais) ? "Brasil" : e.Pais,
                    Principal = e.Principal
                });
            }
        }

        if (dto.Contatos != null && dto.Contatos.Any())
        {
            foreach (var c in dto.Contatos)
            {
                mailing.Contatos.Add(new MailingContato
                {
                    Nome = c.Nome,
                    Cargo = c.Cargo,
                    TelComercial = c.TelComercial,
                    Celular = c.Celular,
                    Email = c.Email,
                    CP = c.CP,
                    VE = c.VE,
                    FI = c.FI,
                    FAT = c.FAT
                });
            }
        }

        if (dto.Preferencias != null && dto.Preferencias.Any())
        {
            foreach (var p in dto.Preferencias)
            {
                mailing.Preferencias.Add(new MailingPreferencia
                {
                    GrupoId = p.GrupoId,
                    GrupoNome = p.GrupoNome,
                    SubGrupoId = p.SubGrupoId,
                    SubGrupoNome = p.SubGrupoNome
                });
            }
        }

        if (dto.Cnaes != null && dto.Cnaes.Any())
        {
            foreach (var c in dto.Cnaes)
            {
                mailing.Cnaes.Add(new MailingCnae
                {
                    Tipo = c.Tipo,
                    CnaeCodigo = c.CnaeCodigo,
                    CnaeDescricao = c.CnaeDescricao
                });
            }
        }

        if (dto.Acoes != null && dto.Acoes.Any())
        {
            foreach (var a in dto.Acoes)
            {
                mailing.Acoes.Add(new MailingAcao
                {
                    Data = a.Data,
                    TipoAcao = a.TipoAcao,
                    Acao = a.Acao,
                    Resultado = a.Resultado,
                    Justificativa = a.Justificativa,
                    UsuarioNome = a.UsuarioNome ?? _currentUser.UsuarioNome
                });
            }
        }

        if (dto.FollowUps != null && dto.FollowUps.Any())
        {
            foreach (var f in dto.FollowUps)
            {
                mailing.FollowUps.Add(new MailingFollowUp
                {
                    DataRetorno = f.DataRetorno,
                    UsuarioId = f.UsuarioId ?? _currentUser.UsuarioId,
                    UsuarioNome = f.UsuarioNome ?? _currentUser.UsuarioNome,
                    Assunto = f.Assunto,
                    Descricao = f.Descricao,
                    Encerrado = f.Encerrado,
                    DataEncerramento = f.DataEncerramento
                });
            }
        }

        if (dto.DadosBancarios != null && dto.DadosBancarios.Any())
        {
            foreach (var d in dto.DadosBancarios)
            {
                mailing.DadosBancarios.Add(new MailingDadoBancario
                {
                    Status = d.Status,
                    Tipo = d.Tipo,
                    BancoCodigo = d.BancoCodigo,
                    BancoNome = d.BancoNome,
                    Agencia = d.Agencia,
                    DigitoAgencia = d.DigitoAgencia,
                    Conta = d.Conta,
                    DigitoConta = d.DigitoConta,
                    Favorecido = d.Favorecido,
                    CnpjCpf = d.CnpjCpf,
                    ChavePix = d.ChavePix
                });
            }
        }

        if (dto.Veiculos != null && dto.Veiculos.Any())
        {
            foreach (var v in dto.Veiculos)
            {
                mailing.Veiculos.Add(new MailingVeiculo
                {
                    TipoVeiculo = v.TipoVeiculo,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Placa = v.Placa,
                    Antt = v.Antt,
                    Estado = v.Estado,
                    Cidade = v.Cidade,
                    TaraKg = v.TaraKg,
                    CapacidadeKg = v.CapacidadeKg
                });
            }
        }

        if (dto.Regioes != null && dto.Regioes.Any())
        {
            foreach (var r in dto.Regioes)
            {
                mailing.Regioes.Add(new MailingRegiao
                {
                    Estado = r.Estado,
                    CidadeNome = r.CidadeNome,
                    PrazoDias = r.PrazoDias,
                    ValorFreteKg = r.ValorFreteKg
                });
            }
        }

        if (dto.Documentos != null && dto.Documentos.Any())
        {
            foreach (var doc in dto.Documentos)
            {
                mailing.Documentos.Add(new MailingDocumento
                {
                    Descricao = doc.Descricao,
                    UsuarioId = doc.UsuarioId ?? _currentUser.UsuarioId,
                    UsuarioNome = doc.UsuarioNome ?? _currentUser.UsuarioNome,
                    DataHora = DateTime.UtcNow,
                    NomeArquivo = doc.NomeArquivo,
                    TipoConteudo = doc.TipoConteudo,
                    TamanhoBytes = doc.TamanhoBytes,
                    StoragePath = doc.StoragePath
                });
            }
        }

        if (dto.Faturamento != null)
        {
            mailing.Faturamento = new MailingFaturamento
            {
                ListaPrecoId = dto.Faturamento.ListaPrecoId,
                ListaPrecoNome = dto.Faturamento.ListaPrecoNome,
                FormaPagtoId = dto.Faturamento.FormaPagtoId,
                FormaPagtoNome = dto.Faturamento.FormaPagtoNome,
                CentroCustoId = dto.Faturamento.CentroCustoId,
                CentroCustoNome = dto.Faturamento.CentroCustoNome,
                ComissaoPct = dto.Faturamento.ComissaoPct,
                TransportadoraId = dto.Faturamento.TransportadoraId,
                TransportadoraNome = dto.Faturamento.TransportadoraNome,
                ValorFrete = dto.Faturamento.ValorFrete,
                LimiteCredito = dto.Faturamento.LimiteCredito,
                Bloqueado = dto.Faturamento.Bloqueado,
                MotivoBloqueio = dto.Faturamento.MotivoBloqueio,
                DiaPagamento = dto.Faturamento.DiaPagamento,
                UsarOutroCadastroNF = dto.Faturamento.UsarOutroCadastroNF,
                MailingNFId = dto.Faturamento.MailingNFId
            };
        }

        _context.Mailings.Add(mailing);
        
        // Audit log
        _context.AuditLogs.Add(new AuditLog
        {
            DataHora = DateTime.UtcNow,
            UsuarioNome = _currentUser.UsuarioNome ?? "Sistema",
            Modulo = "Comercial",
            Entidade = "Mailing",
            Operacao = "Create",
            RegistroId = dto.RazaoSocial ?? dto.NomeCompleto ?? "Novo",
            Detalhes = $"Criação de registro {dto.TipoPessoa} - {(dto.TipoPessoa == Domain.Enums.TipoPessoa.Juridica ? dto.RazaoSocial : dto.NomeCompleto)}"
        });

        await _context.SaveChangesAsync(cancellationToken);
        return mailing.Id;
    }
}

public record UpdateMailingCommand(MailingUpdateDto Dto) : IRequest<bool>;

public class UpdateMailingHandler : IRequestHandler<UpdateMailingCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateMailingHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(UpdateMailingCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var mailing = await _context.Mailings
            .Include(x => x.Enderecos)
            .Include(x => x.Contatos)
            .Include(x => x.Preferencias)
            .Include(x => x.Cnaes)
            .Include(x => x.Acoes)
            .Include(x => x.FollowUps)
            .Include(x => x.DadosBancarios)
            .Include(x => x.Veiculos)
            .Include(x => x.Regioes)
            .Include(x => x.Documentos)
            .Include(x => x.Faturamento)
            .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

        if (mailing == null) return false;

        mailing.Inativo = dto.Inativo;
        mailing.IsCliente = dto.IsCliente;
        mailing.IsFornecedor = dto.IsFornecedor;
        mailing.IsTransportadora = dto.IsTransportadora;
        mailing.IsIntermediador = dto.IsIntermediador;
        mailing.IsFuncionario = dto.IsFuncionario;
        mailing.VendedorId = dto.VendedorId;
        mailing.TipoPessoa = dto.TipoPessoa;
        mailing.RazaoSocial = dto.RazaoSocial;
        mailing.NomeFantasia = dto.NomeFantasia;
        mailing.NomeCompleto = dto.NomeCompleto;
        mailing.Cnpj = dto.Cnpj;
        mailing.Cpf = dto.Cpf;
        mailing.Ie = dto.Ie;
        mailing.Im = dto.Im;
        mailing.Rne = dto.Rne;
        mailing.Rg = dto.Rg;
        mailing.IndIe = dto.IndIe;
        mailing.TipoConsumidor = dto.TipoConsumidor;
        mailing.RegimeTributario = dto.RegimeTributario;
        mailing.Alertas = dto.Alertas;
        mailing.Observacao = dto.Observacao;
        mailing.Ranqueamento = dto.Ranqueamento;
        mailing.Potencial = dto.Potencial;
        mailing.Origem = dto.Origem;
        mailing.ToleranciaProducao = dto.ToleranciaProducao;
        mailing.AtualizadoEm = DateTime.UtcNow;
        mailing.AtualizadoPor = _currentUser.UsuarioNome ?? "Sistema";

        // Update Enderecos
        _context.MailingEnderecos.RemoveRange(mailing.Enderecos);
        if (dto.Enderecos != null)
        {
            foreach (var e in dto.Enderecos)
            {
                mailing.Enderecos.Add(new MailingEndereco
                {
                    TipoEnd = e.TipoEnd,
                    Cep = e.Cep,
                    Logradouro = e.Logradouro,
                    Numero = e.Numero,
                    Complemento = e.Complemento,
                    Bairro = e.Bairro,
                    Cidade = e.Cidade,
                    Estado = e.Estado,
                    Pais = string.IsNullOrWhiteSpace(e.Pais) ? "Brasil" : e.Pais,
                    Principal = e.Principal
                });
            }
        }

        // Update Contatos
        _context.MailingContatos.RemoveRange(mailing.Contatos);
        if (dto.Contatos != null)
        {
            foreach (var c in dto.Contatos)
            {
                mailing.Contatos.Add(new MailingContato
                {
                    Nome = c.Nome,
                    Cargo = c.Cargo,
                    TelComercial = c.TelComercial,
                    Celular = c.Celular,
                    Email = c.Email,
                    CP = c.CP,
                    VE = c.VE,
                    FI = c.FI,
                    FAT = c.FAT
                });
            }
        }

        // Update Preferencias
        _context.MailingPreferencias.RemoveRange(mailing.Preferencias);
        if (dto.Preferencias != null)
        {
            foreach (var p in dto.Preferencias)
            {
                mailing.Preferencias.Add(new MailingPreferencia
                {
                    GrupoId = p.GrupoId,
                    GrupoNome = p.GrupoNome,
                    SubGrupoId = p.SubGrupoId,
                    SubGrupoNome = p.SubGrupoNome
                });
            }
        }

        // Update Cnaes
        _context.MailingCnaes.RemoveRange(mailing.Cnaes);
        if (dto.Cnaes != null)
        {
            foreach (var c in dto.Cnaes)
            {
                mailing.Cnaes.Add(new MailingCnae
                {
                    Tipo = c.Tipo,
                    CnaeCodigo = c.CnaeCodigo,
                    CnaeDescricao = c.CnaeDescricao
                });
            }
        }

        // Update Acoes
        _context.MailingAcoes.RemoveRange(mailing.Acoes);
        if (dto.Acoes != null)
        {
            foreach (var a in dto.Acoes)
            {
                mailing.Acoes.Add(new MailingAcao
                {
                    Data = a.Data,
                    TipoAcao = a.TipoAcao,
                    Acao = a.Acao,
                    Resultado = a.Resultado,
                    Justificativa = a.Justificativa,
                    UsuarioNome = a.UsuarioNome ?? _currentUser.UsuarioNome
                });
            }
        }

        // Update FollowUps
        _context.MailingFollowUps.RemoveRange(mailing.FollowUps);
        if (dto.FollowUps != null)
        {
            foreach (var f in dto.FollowUps)
            {
                mailing.FollowUps.Add(new MailingFollowUp
                {
                    DataRetorno = f.DataRetorno,
                    UsuarioId = f.UsuarioId ?? _currentUser.UsuarioId,
                    UsuarioNome = f.UsuarioNome ?? _currentUser.UsuarioNome,
                    Assunto = f.Assunto,
                    Descricao = f.Descricao,
                    Encerrado = f.Encerrado,
                    DataEncerramento = f.DataEncerramento
                });
            }
        }

        // Update Dados Bancários
        _context.MailingDadosBancarios.RemoveRange(mailing.DadosBancarios);
        if (dto.DadosBancarios != null)
        {
            foreach (var d in dto.DadosBancarios)
            {
                mailing.DadosBancarios.Add(new MailingDadoBancario
                {
                    Status = d.Status,
                    Tipo = d.Tipo,
                    BancoCodigo = d.BancoCodigo,
                    BancoNome = d.BancoNome,
                    Agencia = d.Agencia,
                    DigitoAgencia = d.DigitoAgencia,
                    Conta = d.Conta,
                    DigitoConta = d.DigitoConta,
                    Favorecido = d.Favorecido,
                    CnpjCpf = d.CnpjCpf,
                    ChavePix = d.ChavePix
                });
            }
        }

        // Update Veiculos
        _context.MailingVeiculos.RemoveRange(mailing.Veiculos);
        if (dto.Veiculos != null)
        {
            foreach (var v in dto.Veiculos)
            {
                mailing.Veiculos.Add(new MailingVeiculo
                {
                    TipoVeiculo = v.TipoVeiculo,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Placa = v.Placa,
                    Antt = v.Antt,
                    Estado = v.Estado,
                    Cidade = v.Cidade,
                    TaraKg = v.TaraKg,
                    CapacidadeKg = v.CapacidadeKg
                });
            }
        }

        // Update Regioes
        _context.MailingRegioes.RemoveRange(mailing.Regioes);
        if (dto.Regioes != null)
        {
            foreach (var r in dto.Regioes)
            {
                mailing.Regioes.Add(new MailingRegiao
                {
                    Estado = r.Estado,
                    CidadeNome = r.CidadeNome,
                    PrazoDias = r.PrazoDias,
                    ValorFreteKg = r.ValorFreteKg
                });
            }
        }

        // Update Faturamento
        if (dto.Faturamento != null)
        {
            if (mailing.Faturamento == null)
            {
                mailing.Faturamento = new MailingFaturamento { MailingId = mailing.Id };
                _context.MailingFaturamentos.Add(mailing.Faturamento);
            }

            mailing.Faturamento.ListaPrecoId = dto.Faturamento.ListaPrecoId;
            mailing.Faturamento.ListaPrecoNome = dto.Faturamento.ListaPrecoNome;
            mailing.Faturamento.FormaPagtoId = dto.Faturamento.FormaPagtoId;
            mailing.Faturamento.FormaPagtoNome = dto.Faturamento.FormaPagtoNome;
            mailing.Faturamento.CentroCustoId = dto.Faturamento.CentroCustoId;
            mailing.Faturamento.CentroCustoNome = dto.Faturamento.CentroCustoNome;
            mailing.Faturamento.ComissaoPct = dto.Faturamento.ComissaoPct;
            mailing.Faturamento.TransportadoraId = dto.Faturamento.TransportadoraId;
            mailing.Faturamento.TransportadoraNome = dto.Faturamento.TransportadoraNome;
            mailing.Faturamento.ValorFrete = dto.Faturamento.ValorFrete;
            mailing.Faturamento.LimiteCredito = dto.Faturamento.LimiteCredito;
            mailing.Faturamento.Bloqueado = dto.Faturamento.Bloqueado;
            mailing.Faturamento.MotivoBloqueio = dto.Faturamento.MotivoBloqueio;
            mailing.Faturamento.DiaPagamento = dto.Faturamento.DiaPagamento;
            mailing.Faturamento.UsarOutroCadastroNF = dto.Faturamento.UsarOutroCadastroNF;
            mailing.Faturamento.MailingNFId = dto.Faturamento.MailingNFId;
        }

        // Audit Log
        _context.AuditLogs.Add(new AuditLog
        {
            DataHora = DateTime.UtcNow,
            UsuarioNome = _currentUser.UsuarioNome ?? "Sistema",
            Modulo = "Comercial",
            Entidade = "Mailing",
            Operacao = "Update",
            RegistroId = mailing.Id.ToString(),
            Detalhes = $"Atualização de registro #{mailing.Id}"
        });

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record InativarMailingCommand(int Id) : IRequest<bool>;

public class InativarMailingHandler : IRequestHandler<InativarMailingCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public InativarMailingHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(InativarMailingCommand request, CancellationToken cancellationToken)
    {
        var m = await _context.Mailings.FindAsync(new object[] { request.Id }, cancellationToken);
        if (m == null) return false;

        m.Inativo = !m.Inativo;
        m.AtualizadoEm = DateTime.UtcNow;
        m.AtualizadoPor = _currentUser.UsuarioNome ?? "Sistema";

        _context.AuditLogs.Add(new AuditLog
        {
            DataHora = DateTime.UtcNow,
            UsuarioNome = _currentUser.UsuarioNome ?? "Sistema",
            Modulo = "Comercial",
            Entidade = "Mailing",
            Operacao = m.Inativo ? "Inactivate" : "Reactivate",
            RegistroId = m.Id.ToString(),
            Detalhes = $"Mailing #{m.Id} alterado para Inativo={m.Inativo}"
        });

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record DeleteMailingCommand(int Id) : IRequest<bool>;

public class DeleteMailingHandler : IRequestHandler<DeleteMailingCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteMailingHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(DeleteMailingCommand request, CancellationToken cancellationToken)
    {
        var m = await _context.Mailings.FindAsync(new object[] { request.Id }, cancellationToken);
        if (m == null) return false;

        m.Inativo = true; // Soft delete
        _context.AuditLogs.Add(new AuditLog
        {
            DataHora = DateTime.UtcNow,
            UsuarioNome = _currentUser.UsuarioNome ?? "Sistema",
            Modulo = "Comercial",
            Entidade = "Mailing",
            Operacao = "Delete",
            RegistroId = m.Id.ToString(),
            Detalhes = $"Mailing #{m.Id} excluído logicamente (soft delete)"
        });

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
