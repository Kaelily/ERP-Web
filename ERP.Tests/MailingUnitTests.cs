using ERP.Application.DTOs.Auth;
using ERP.Application.DTOs.Mailing;
using ERP.Application.Features.Auth;
using ERP.Application.Features.Mailing.Commands;
using ERP.Application.Features.Mailing.Queries;
using ERP.Application.Interfaces;
using ERP.Application.Validators;
using ERP.Domain.Entities.Auth;
using ERP.Domain.Entities.Mailings;
using ERP.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ERP.Tests;

public class MailingValidatorTests
{
    private readonly MailingCreateValidator _validator = new();

    [Fact]
    public void Validator_Should_Fail_When_PJ_Without_RazaoSocial()
    {
        var dto = new MailingCreateDto
        {
            TipoPessoa = TipoPessoa.Juridica,
            RazaoSocial = "",
            IndIe = IndIe.ContribuinteICMS
        };

        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RazaoSocial");
    }

    [Fact]
    public void Validator_Should_Fail_When_PF_Without_NomeCompleto()
    {
        var dto = new MailingCreateDto
        {
            TipoPessoa = TipoPessoa.Fisica,
            NomeCompleto = "",
            IndIe = IndIe.NaoContribuinte
        };

        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NomeCompleto");
    }

    [Fact]
    public void Validator_Should_Pass_For_Valid_PJ()
    {
        var dto = new MailingCreateDto
        {
            TipoPessoa = TipoPessoa.Juridica,
            RazaoSocial = "ACME INDUSTRIA E COMERCIO LTDA",
            Cnpj = "12.345.678/0001-90",
            IndIe = IndIe.ContribuinteICMS,
            Enderecos = new List<MailingEnderecoDto>
            {
                new()
                {
                    Cep = "01310-100",
                    Logradouro = "Av Paulista",
                    Numero = "1000",
                    Bairro = "Bela Vista",
                    Cidade = "São Paulo",
                    Estado = "SP"
                }
            }
        };

        var result = _validator.Validate(dto);
        result.IsValid.Should().BeTrue();
    }
}

public class MailingHandlerTests
{
    private AppTestDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppTestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppTestDbContext(options);
    }

    [Fact]
    public async Task CreateMailingHandler_Should_Persist_Mailing_With_Children()
    {
        var context = CreateDbContext();
        var mockUser = new Mock<ICurrentUserService>();
        mockUser.Setup(u => u.UsuarioNome).Returns("Admin Test");

        var handler = new CreateMailingHandler(context, mockUser.Object);

        var dto = new MailingCreateDto
        {
            TipoPessoa = TipoPessoa.Juridica,
            RazaoSocial = "Metalúrgica Nova Era Ltda",
            Cnpj = "11.222.333/0001-44",
            IndIe = IndIe.ContribuinteICMS,
            Enderecos = new List<MailingEnderecoDto>
            {
                new() { TipoEnd = TipoEndereco.Faturamento, Cep = "14000-000", Logradouro = "Rua Teste", Numero = "10", Bairro = "Centro", Cidade = "Ribeirão", Estado = "SP", Principal = true }
            },
            Contatos = new List<MailingContatoDto>
            {
                new() { Nome = "Carlos Comprador", TelComercial = "1699999999", CP = true }
            },
            Faturamento = new MailingFaturamentoDto
            {
                LimiteCredito = 100000,
                Bloqueado = false
            }
        };

        var id = await handler.Handle(new CreateMailingCommand(dto), CancellationToken.None);

        id.Should().BeGreaterThan(0);
        var created = await context.Mailings
            .Include(m => m.Enderecos)
            .Include(m => m.Contatos)
            .Include(m => m.Faturamento)
            .FirstOrDefaultAsync(m => m.Id == id);

        created.Should().NotBeNull();
        created!.RazaoSocial.Should().Be("Metalúrgica Nova Era Ltda");
        created.Enderecos.Should().HaveCount(1);
        created.Contatos.Should().HaveCount(1);
        created.Faturamento.Should().NotBeNull();
        created.Faturamento!.LimiteCredito.Should().Be(100000);
    }

    [Fact]
    public async Task InativarMailingHandler_Should_Toggle_Inactive_Status()
    {
        var context = CreateDbContext();
        var mockUser = new Mock<ICurrentUserService>();
        mockUser.Setup(u => u.UsuarioNome).Returns("Admin Test");

        var mailing = new Mailing
        {
            RazaoSocial = "Empresa Ativa Ltda",
            Inativo = false,
            IndIe = IndIe.ContribuinteICMS
        };
        context.Mailings.Add(mailing);
        await context.SaveChangesAsync();

        var handler = new InativarMailingHandler(context, mockUser.Object);
        var result = await handler.Handle(new InativarMailingCommand(mailing.Id), CancellationToken.None);

        result.Should().BeTrue();
        var updated = await context.Mailings.FindAsync(mailing.Id);
        updated!.Inativo.Should().BeTrue();
    }
}

public class AppTestDbContext : ERP.Infrastructure.Persistence.AppDbContext
{
    public AppTestDbContext(DbContextOptions<AppTestDbContext> options) : base(ChangeOptionsType(options))
    {
    }

    private static DbContextOptions<ERP.Infrastructure.Persistence.AppDbContext> ChangeOptionsType(DbContextOptions<AppTestDbContext> options)
    {
        return new DbContextOptionsBuilder<ERP.Infrastructure.Persistence.AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }
}
