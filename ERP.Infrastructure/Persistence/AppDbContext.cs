using ERP.Application.Interfaces;
using ERP.Domain.Entities.Auth;
using ERP.Domain.Entities.Comercial;
using ERP.Domain.Entities.Estoque;
using ERP.Domain.Entities.Faturamento;
using ERP.Domain.Entities.Financeiro;
using ERP.Domain.Entities.Mailings;
using ERP.Domain.Entities.Sistema;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Perfil> Perfis => Set<Perfil>();
    public DbSet<PerfilModulo> PerfilModulos => Set<PerfilModulo>();
    public DbSet<PerfilPermissao> PerfilPermissoes => Set<PerfilPermissao>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Mailing> Mailings => Set<Mailing>();
    public DbSet<MailingEndereco> MailingEnderecos => Set<MailingEndereco>();
    public DbSet<MailingContato> MailingContatos => Set<MailingContato>();
    public DbSet<MailingPreferencia> MailingPreferencias => Set<MailingPreferencia>();
    public DbSet<MailingCnae> MailingCnaes => Set<MailingCnae>();
    public DbSet<MailingAcao> MailingAcoes => Set<MailingAcao>();
    public DbSet<MailingFollowUp> MailingFollowUps => Set<MailingFollowUp>();
    public DbSet<MailingDadoBancario> MailingDadosBancarios => Set<MailingDadoBancario>();
    public DbSet<MailingVeiculo> MailingVeiculos => Set<MailingVeiculo>();
    public DbSet<MailingRegiao> MailingRegioes => Set<MailingRegiao>();
    public DbSet<MailingDocumento> MailingDocumentos => Set<MailingDocumento>();
    public DbSet<MailingFaturamento> MailingFaturamentos => Set<MailingFaturamento>();

    public DbSet<OportunidadeCrm> OportunidadesCrm => Set<OportunidadeCrm>();
    public DbSet<OrdemServico> OrdensServico => Set<OrdemServico>();
    public DbSet<PedidoVenda> PedidosVenda => Set<PedidoVenda>();
    public DbSet<PedidoVendaItem> PedidoVendaItens => Set<PedidoVendaItem>();

    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<LocalEstoque> LocaisEstoque => Set<LocalEstoque>();
    public DbSet<Lote> Lotes => Set<Lote>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();

    public DbSet<ContaBancaria> ContasBancarias => Set<ContaBancaria>();
    public DbSet<CentroCusto> CentrosCusto => Set<CentroCusto>();
    public DbSet<PlanoConta> PlanosContas => Set<PlanoConta>();
    public DbSet<TituloFinanceiro> TitulosFinanceiros => Set<TituloFinanceiro>();

    public DbSet<TributacaoConfig> TributacoesConfig => Set<TributacaoConfig>();
    public DbSet<NotaFiscal> NotasFiscais => Set<NotaFiscal>();
    public DbSet<NotaFiscalItem> NotaFiscalItens => Set<NotaFiscalItem>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<ParametrosSistema> Parametros => Set<ParametrosSistema>();
    public DbSet<Feriado> Feriados => Set<Feriado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mailing Relationships
        modelBuilder.Entity<Mailing>()
            .HasMany(m => m.Enderecos)
            .WithOne(e => e.Mailing)
            .HasForeignKey(e => e.MailingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Mailing>()
            .HasMany(m => m.Contatos)
            .WithOne(c => c.Mailing)
            .HasForeignKey(c => c.MailingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Mailing>()
            .HasMany(m => m.Preferencias)
            .WithOne(p => p.Mailing)
            .HasForeignKey(p => p.MailingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Mailing>()
            .HasMany(m => m.Cnaes)
            .WithOne(c => c.Mailing)
            .HasForeignKey(c => c.MailingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Mailing>()
            .HasMany(m => m.Acoes)
            .WithOne(a => a.Mailing)
            .HasForeignKey(a => a.MailingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Mailing>()
            .HasMany(m => m.FollowUps)
            .WithOne(f => f.Mailing)
            .HasForeignKey(f => f.MailingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Mailing>()
            .HasMany(m => m.DadosBancarios)
            .WithOne(d => d.Mailing)
            .HasForeignKey(d => d.MailingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Mailing>()
            .HasMany(m => m.Veiculos)
            .WithOne(v => v.Mailing)
            .HasForeignKey(v => v.MailingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Mailing>()
            .HasMany(m => m.Regioes)
            .WithOne(r => r.Mailing)
            .HasForeignKey(r => r.MailingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Mailing>()
            .HasMany(m => m.Documentos)
            .WithOne(d => d.Mailing)
            .HasForeignKey(d => d.MailingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Mailing>()
            .HasOne(m => m.Faturamento)
            .WithOne(f => f.Mailing)
            .HasForeignKey<MailingFaturamento>(f => f.MailingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Pedido Venda
        modelBuilder.Entity<PedidoVenda>()
            .HasMany(p => p.Itens)
            .WithOne(i => i.PedidoVenda)
            .HasForeignKey(i => i.PedidoVendaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Nota Fiscal
        modelBuilder.Entity<NotaFiscal>()
            .HasMany(n => n.Itens)
            .WithOne(i => i.NotaFiscal)
            .HasForeignKey(i => i.NotaFiscalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
