using ERP.Domain.Entities.Auth;
using ERP.Domain.Entities.Comercial;
using ERP.Domain.Entities.Estoque;
using ERP.Domain.Entities.Faturamento;
using ERP.Domain.Entities.Financeiro;
using ERP.Domain.Entities.Mailings;
using ERP.Domain.Entities.Sistema;
using ERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Perfis.AnyAsync())
        {
            return; // Already initialized
        }

        // 1. Perfis e Permissões
        var perfilAdmin = new Perfil
        {
            Nome = "Administrador",
            Descricao = "Acesso irrestrito a todos os módulos e configurações de sistema"
        };
        foreach (ModuloSistema modulo in Enum.GetValues<ModuloSistema>())
        {
            perfilAdmin.Modulos.Add(new PerfilModulo
            {
                Modulo = modulo,
                NivelAcesso = NivelAcesso.Full,
                PodeLer = true,
                PodeCriar = true,
                PodeEditar = true,
                PodeExcluir = true
            });
        }

        var perfilComercial = new Perfil
        {
            Nome = "Comercial",
            Descricao = "Acesso completo a Vendas, CRM, Mailing e Contratos"
        };
        perfilComercial.Modulos.Add(new PerfilModulo { Modulo = ModuloSistema.Comercial, NivelAcesso = NivelAcesso.CRUD, PodeLer = true, PodeCriar = true, PodeEditar = true, PodeExcluir = true });
        perfilComercial.Modulos.Add(new PerfilModulo { Modulo = ModuloSistema.CRM, NivelAcesso = NivelAcesso.CRUD, PodeLer = true, PodeCriar = true, PodeEditar = true, PodeExcluir = true });
        perfilComercial.Modulos.Add(new PerfilModulo { Modulo = ModuloSistema.Contratos, NivelAcesso = NivelAcesso.CRUD, PodeLer = true, PodeCriar = true, PodeEditar = true, PodeExcluir = true });

        var perfilFinanceiro = new Perfil
        {
            Nome = "Financeiro",
            Descricao = "Acesso a Contas a Pagar/Receber, Bancos e Faturamento"
        };
        perfilFinanceiro.Modulos.Add(new PerfilModulo { Modulo = ModuloSistema.Financeiro, NivelAcesso = NivelAcesso.CRUD, PodeLer = true, PodeCriar = true, PodeEditar = true, PodeExcluir = true });
        perfilFinanceiro.Modulos.Add(new PerfilModulo { Modulo = ModuloSistema.Faturamento, NivelAcesso = NivelAcesso.CRUD, PodeLer = true, PodeCriar = true, PodeEditar = true, PodeExcluir = true });

        context.Perfis.AddRange(perfilAdmin, perfilComercial, perfilFinanceiro);
        await context.SaveChangesAsync();

        // 2. Usuários
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        var userAdmin = new Usuario
        {
            Nome = "Administrador Geral",
            Email = "admin@erp.com.br",
            SenhaHash = senhaHash,
            PerfilId = perfilAdmin.Id,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };
        var userComercial = new Usuario
        {
            Nome = "João Silva (Vendedor)",
            Email = "joao.vendedor@erp.com.br",
            SenhaHash = senhaHash,
            PerfilId = perfilComercial.Id,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };
        var userFinanceiro = new Usuario
        {
            Nome = "Maria Santos (Financeiro)",
            Email = "maria.financeiro@erp.com.br",
            SenhaHash = senhaHash,
            PerfilId = perfilFinanceiro.Id,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        context.Usuarios.AddRange(userAdmin, userComercial, userFinanceiro);
        await context.SaveChangesAsync();

        // 3. Mailing Vendedor (Funcionário)
        var vendedorMailing = new Domain.Entities.Mailings.Mailing
        {
            IsFuncionario = true,
            TipoPessoa = TipoPessoa.Fisica,
            NomeCompleto = "João Silva Vendedor",
            Cpf = "123.456.789-00",
            Rg = "12.345.678-9",
            IndIe = IndIe.NaoContribuinte,
            Potencial = PotencialMailing.Alto,
            Observacao = "Vendedor interno senior - Região Sudeste",
            CriadoEm = DateTime.UtcNow
        };
        context.Mailings.Add(vendedorMailing);
        await context.SaveChangesAsync();

        // 4. Mailing Clientes / Fornecedores / Transportadoras
        var cliente1 = new Domain.Entities.Mailings.Mailing
        {
            IsCliente = true,
            IsFornecedor = true,
            TipoPessoa = TipoPessoa.Juridica,
            RazaoSocial = "INDÚSTRIA METALÚRGICA ALUMÍNIO BRASIL LTDA",
            NomeFantasia = "ALUMÍNIO BRASIL",
            Cnpj = "12.345.678/0001-90",
            Ie = "123.456.789.111",
            Im = "987654",
            IndIe = IndIe.ContribuinteICMS,
            RegimeTributario = RegimeTributario.LucroReal,
            VendedorId = vendedorMailing.Id,
            Ranqueamento = "A+",
            Potencial = PotencialMailing.Alto,
            Origem = "Feira Industrial 2025",
            ToleranciaProducao = 5.00m,
            Alertas = "Cliente VIP. Exige laudo técnico de qualidade em todas as entregas.",
            Observacao = "Faturamento quinzenal. Pagamento sempre em dia.",
            CriadoEm = DateTime.UtcNow.AddMonths(-10),

            Enderecos = new List<MailingEndereco>
            {
                new()
                {
                    TipoEnd = TipoEndereco.Faturamento,
                    Cep = "04571-010",
                    Logradouro = "Avenida Engenheiro Luís Carlos Berrini",
                    Numero = "1000",
                    Complemento = "Andar 14",
                    Bairro = "Cidade Monções",
                    Cidade = "São Paulo",
                    Estado = "SP",
                    Pais = "Brasil",
                    Principal = true
                },
                new()
                {
                    TipoEnd = TipoEndereco.RetiradaEntrega,
                    Cep = "13080-000",
                    Logradouro = "Rodovia Dom Pedro I",
                    Numero = "Km 135",
                    Bairro = "Parque Rural",
                    Cidade = "Campinas",
                    Estado = "SP",
                    Pais = "Brasil",
                    Principal = false
                }
            },
            Contatos = new List<MailingContato>
            {
                new()
                {
                    Nome = "Roberto Mendes",
                    Cargo = "Diretor de Suprimentos",
                    TelComercial = "(11) 3500-9000",
                    Celular = "(11) 98765-4321",
                    Email = "roberto.mendes@aluminiobrasil.com.br",
                    CP = true,
                    FAT = true
                },
                new()
                {
                    Nome = "Fernanda Lima",
                    Cargo = "Gerente Financeira",
                    TelComercial = "(11) 3500-9002",
                    Email = "financeiro@aluminiobrasil.com.br",
                    FI = true
                }
            },
            Preferencias = new List<MailingPreferencia>
            {
                new() { GrupoId = 1, GrupoNome = "Perfis de Alumínio", SubGrupoId = 10, SubGrupoNome = "Extrudados" },
                new() { GrupoId = 2, GrupoNome = "Chapas Industriais", SubGrupoId = 21, SubGrupoNome = "Chapas Liga 6060" }
            },
            Cnaes = new List<MailingCnae>
            {
                new() { Tipo = TipoCnae.Principal, CnaeCodigo = "24.41-5-01", CnaeDescricao = "Metalurgia dos metais preciosos e alumínio" },
                new() { Tipo = TipoCnae.Secundario, CnaeCodigo = "25.39-0-01", CnaeDescricao = "Serviços de usinagem, tornearia e solda" }
            },
            Acoes = new List<MailingAcao>
            {
                new() { Data = DateTime.UtcNow.AddDays(-20), TipoAcao = "Visita Comercial", Acao = "Apresentação da nova linha de perfis leves", Resultado = "Cliente demonstrou interesse em fechar cotação de 50 toneladas.", UsuarioNome = "João Silva" },
                new() { Data = DateTime.UtcNow.AddDays(-5), TipoAcao = "Envio de Proposta", Acao = "Envio da proposta com condição 30/60 DDL", Resultado = "Aguardando aprovação da diretoria", UsuarioNome = "João Silva" }
            },
            FollowUps = new List<MailingFollowUp>
            {
                new() { DataRetorno = DateTime.UtcNow.AddDays(2), Assunto = "Follow-up Proposta Comercial 2026", Descricao = "Ligar para o Roberto Mendes para alinhar prazos de entrega do 1º lote.", UsuarioNome = "João Silva", Encerrado = false }
            },
            DadosBancarios = new List<MailingDadoBancario>
            {
                new() { BancoCodigo = "341", BancoNome = "Itaú Unibanco S.A.", Agencia = "0345", Conta = "98234-1", Favorecido = "INDÚSTRIA METALÚRGICA ALUMÍNIO BRASIL", CnpjCpf = "12.345.678/0001-90", ChavePix = "financeiro@aluminiobrasil.com.br", Status = "Ativo" }
            },
            Faturamento = new MailingFaturamento
            {
                ListaPrecoId = 1,
                ListaPrecoNome = "Tabela Indústria A+",
                FormaPagtoId = 2,
                FormaPagtoNome = "Boleto 30/60/90 Dias",
                CentroCustoId = 1,
                CentroCustoNome = "Vendas Nacional",
                ComissaoPct = 3.50m,
                LimiteCredito = 500000.00m,
                ValorFrete = 150.00m,
                Bloqueado = false,
                DiaPagamento = 10
            }
        };

        var transportadora = new Domain.Entities.Mailings.Mailing
        {
            IsTransportadora = true,
            TipoPessoa = TipoPessoa.Juridica,
            RazaoSocial = "TRANSLOG TRANSPORTES E LOGÍSTICA S/A",
            NomeFantasia = "TRANSLOG EXPRESS",
            Cnpj = "98.765.432/0001-10",
            Ie = "987.654.321.000",
            IndIe = IndIe.ContribuinteICMS,
            RegimeTributario = RegimeTributario.LucroReal,
            Potencial = PotencialMailing.Alto,
            CriadoEm = DateTime.UtcNow.AddMonths(-6),

            Enderecos = new List<MailingEndereco>
            {
                new() { TipoEnd = TipoEndereco.Faturamento, Cep = "07170-350", Logradouro = "Via de Acesso Presidente Dutra", Numero = "2500", Bairro = "Cumbica", Cidade = "Guarulhos", Estado = "SP", Principal = true }
            },
            Contatos = new List<MailingContato>
            {
                new() { Nome = "Marcos Frota", Cargo = "Gerente Operacional", TelComercial = "(11) 4004-8899", Email = "operacoes@translog.com.br", CP = true, VE = true }
            },
            Veiculos = new List<MailingVeiculo>
            {
                new() { TipoVeiculo = "Carreta Graneleira", Marca = "Scania", Modelo = "R450", Placa = "BRA2E26", Antt = "12345678", Estado = "SP", Cidade = "Guarulhos", TaraKg = 14500, CapacidadeKg = 32000 },
                new() { TipoVeiculo = "Caminhão Toco", Marca = "Mercedes-Benz", Modelo = "Atego 2426", Placa = "ERP2026", Antt = "87654321", Estado = "SP", Cidade = "São Paulo", TaraKg = 6800, CapacidadeKg = 15000 }
            },
            Regioes = new List<MailingRegiao>
            {
                new() { Estado = "SP", CidadeNome = "Grande São Paulo e Interior", PrazoDias = 1, ValorFreteKg = 0.45m },
                new() { Estado = "MG", CidadeNome = "Belo Horizonte e Região", PrazoDias = 2, ValorFreteKg = 0.85m },
                new() { Estado = "RJ", CidadeNome = "Rio de Janeiro Capital", PrazoDias = 2, ValorFreteKg = 0.78m },
                new() { Estado = "PR", CidadeNome = "Curitiba", PrazoDias = 2, ValorFreteKg = 0.90m }
            }
        };

        var cliente2 = new Domain.Entities.Mailings.Mailing
        {
            IsCliente = true,
            TipoPessoa = TipoPessoa.Juridica,
            RazaoSocial = "SOLUÇÕES CONSTRUTIVAS PAULISTA LTDA",
            NomeFantasia = "CONSTRUTIVA PAULISTA",
            Cnpj = "34.567.890/0001-22",
            Ie = "543.210.987.654",
            IndIe = IndIe.ContribuinteICMS,
            RegimeTributario = RegimeTributario.SimplesNacional,
            VendedorId = vendedorMailing.Id,
            Ranqueamento = "B",
            Potencial = PotencialMailing.Medio,
            CriadoEm = DateTime.UtcNow.AddMonths(-3),
            Enderecos = new List<MailingEndereco>
            {
                new() { TipoEnd = TipoEndereco.Faturamento, Cep = "14020-000", Logradouro = "Avenida Presidente Vargas", Numero = "450", Bairro = "Jardim Santa Ângela", Cidade = "Ribeirão Preto", Estado = "SP", Principal = true }
            },
            Contatos = new List<MailingContato>
            {
                new() { Nome = "Ana Paula Nogueira", Cargo = "Sócia / Compras", TelComercial = "(16) 3600-1122", Celular = "(16) 99123-4567", Email = "anapaula@construtivapaulista.com.br", CP = true, FI = true }
            },
            Faturamento = new MailingFaturamento
            {
                ListaPrecoId = 2,
                ListaPrecoNome = "Tabela Varejo/Construtora",
                LimiteCredito = 150000.00m,
                Bloqueado = false
            }
        };

        context.Mailings.AddRange(cliente1, transportadora, cliente2);
        await context.SaveChangesAsync();

        // 5. Produtos e Estoque
        var prod1 = new Produto { Codigo = "ALU-6060-01", Descricao = "Perfil Alumínio Tubular 50x50x2mm", Unidade = "BARRA", Ncm = "7604.29.00", Grupo = "Perfis Extrudados", PrecoCusto = 85.00m, PrecoVenda = 135.00m, MargemLucroPct = 58.82m, EstoqueAtual = 450, EstoqueMinimo = 50, EstoqueMaximo = 1000, Ativo = true };
        var prod2 = new Produto { Codigo = "ALU-CH-02", Descricao = "Chapa de Alumínio Lisa 2000x1000x1.5mm", Unidade = "CHAPA", Ncm = "7606.12.90", Grupo = "Chapas", PrecoCusto = 190.00m, PrecoVenda = 295.00m, MargemLucroPct = 55.26m, EstoqueAtual = 120, EstoqueMinimo = 20, EstoqueMaximo = 300, Ativo = true };
        var prod3 = new Produto { Codigo = "ALU-CAN-03", Descricao = "Cantoneira de Alumínio Abas Iguais 1x1/8", Unidade = "BARRA", Ncm = "7604.29.00", Grupo = "Perfis", PrecoCusto = 32.00m, PrecoVenda = 55.00m, MargemLucroPct = 71.87m, EstoqueAtual = 800, EstoqueMinimo = 100, EstoqueMaximo = 2000, Ativo = true };
        context.Produtos.AddRange(prod1, prod2, prod3);
        await context.SaveChangesAsync();

        // 6. CRM Kanban Opportunities
        var crm1 = new OportunidadeCrm { MailingId = cliente1.Id, Titulo = "Fornecimento de Perfis Fachada Torre Berrini", ValorEstimado = 185000.00m, Status = StatusCrm.Negociacao, ProbabilidadePct = 80, PrevisaoFechamento = DateTime.UtcNow.AddDays(15), Responsavel = "João Silva", Descricao = "Projeto arquitetônico de 40 andares. Cotação final enviada." };
        var crm2 = new OportunidadeCrm { MailingId = cliente2.Id, Titulo = "Lote de Chapas para Revestimento Residencial", ValorEstimado = 42000.00m, Status = StatusCrm.Proposta, ProbabilidadePct = 50, PrevisaoFechamento = DateTime.UtcNow.AddDays(25), Responsavel = "João Silva", Descricao = "Cliente solicitou desconto para pagamento à vista no PIX." };
        var crm3 = new OportunidadeCrm { MailingId = cliente1.Id, Titulo = "Renovação Anual de Contrato de Tubulares", ValorEstimado = 320000.00m, Status = StatusCrm.Fechado, ProbabilidadePct = 100, PrevisaoFechamento = DateTime.UtcNow.AddDays(-2), Responsavel = "João Silva", Descricao = "Contrato assinado pelo Roberto Mendes." };
        var crm4 = new OportunidadeCrm { MailingId = cliente2.Id, Titulo = "Expansão Shopping Campinas - Esquadrias", ValorEstimado = 75000.00m, Status = StatusCrm.Prospeccao, ProbabilidadePct = 25, PrevisaoFechamento = DateTime.UtcNow.AddDays(45), Responsavel = "João Silva", Descricao = "Primeiro contato realizado após lead gerado no site." };
        context.OportunidadesCrm.AddRange(crm1, crm2, crm3, crm4);

        // 7. Pedidos de Venda
        var ped1 = new PedidoVenda
        {
            Numero = "PV-2026/00101",
            ClienteId = cliente1.Id,
            VendedorId = vendedorMailing.Id,
            TransportadoraId = transportadora.Id,
            Status = StatusPedido.Aprovado,
            DataEmissao = DateTime.UtcNow.AddDays(-5),
            DataPrevisaoEntrega = DateTime.UtcNow.AddDays(5),
            SubTotal = 40500.00m,
            Desconto = 500.00m,
            ValorFrete = 150.00m,
            ValorTotal = 40150.00m,
            CondicaoPagamento = "30/60 DDL",
            Itens = new List<PedidoVendaItem>
            {
                new() { ProdutoId = prod1.Id, ProdutoCodigo = prod1.Codigo, ProdutoDescricao = prod1.Descricao, Unidade = "BARRA", Quantidade = 300, PrecoUnitario = 135.00m, DescontoPct = 0, ValorTotal = 40500.00m }
            }
        };
        context.PedidosVenda.Add(ped1);

        // 8. Financeiro (Contas Bancárias e Títulos)
        var contaItau = new ContaBancaria { NomeConta = "Itaú Operacional Matriz", BancoCodigo = "341", BancoNome = "Banco Itaú S.A.", Agencia = "0057", NumeroConta = "12980-4", SaldoAtual = 284500.00m, Ativo = true };
        var contaBradesco = new ContaBancaria { NomeConta = "Bradesco Cobrança e PIX", BancoCodigo = "237", BancoNome = "Banco Bradesco S.A.", Agencia = "1420", NumeroConta = "44890-0", SaldoAtual = 145200.50m, Ativo = true };
        context.ContasBancarias.AddRange(contaItau, contaBradesco);

        var tit1 = new TituloFinanceiro { Tipo = TipoTitulo.Receber, NumeroDocumento = "DUP-00101/01", MailingId = cliente1.Id, SacadoCedenteNome = cliente1.RazaoSocial, ContaBancaria = contaItau, DataEmissao = DateTime.UtcNow.AddDays(-5), DataVencimento = DateTime.UtcNow.AddDays(25), ValorOriginal = 20075.00m, ValorSaldo = 20075.00m, Status = StatusTitulo.Pendente, FormaPagamento = "Boleto Bancário" };
        var tit2 = new TituloFinanceiro { Tipo = TipoTitulo.Receber, NumeroDocumento = "DUP-00101/02", MailingId = cliente1.Id, SacadoCedenteNome = cliente1.RazaoSocial, ContaBancaria = contaItau, DataEmissao = DateTime.UtcNow.AddDays(-5), DataVencimento = DateTime.UtcNow.AddDays(55), ValorOriginal = 20075.00m, ValorSaldo = 20075.00m, Status = StatusTitulo.Pendente, FormaPagamento = "Boleto Bancário" };
        var titPagar = new TituloFinanceiro { Tipo = TipoTitulo.Pagar, NumeroDocumento = "NF-5489-FRETE", MailingId = transportadora.Id, SacadoCedenteNome = transportadora.RazaoSocial, ContaBancaria = contaItau, DataEmissao = DateTime.UtcNow.AddDays(-3), DataVencimento = DateTime.UtcNow.AddDays(10), ValorOriginal = 3200.00m, ValorSaldo = 3200.00m, Status = StatusTitulo.Pendente, FormaPagamento = "TED/PIX" };
        context.TitulosFinanceiros.AddRange(tit1, tit2, titPagar);

        // 9. Faturamento (Notas Fiscais)
        var nfe = new NotaFiscal
        {
            Numero = "00004589",
            Serie = "1",
            ChaveAcesso = "35260912345678000190550010000045891001234567",
            Modelo = "55",
            DestinatarioId = cliente1.Id,
            DestinatarioNome = cliente1.RazaoSocial!,
            DestinatarioCnpjCpf = cliente1.Cnpj!,
            DataEmissao = DateTime.UtcNow.AddDays(-1),
            DataAutorizacao = DateTime.UtcNow.AddDays(-1).AddMinutes(1),
            ValorProdutos = 40500.00m,
            ValorFrete = 150.00m,
            ValorImpostos = 7290.00m,
            ValorTotal = 40650.00m,
            Status = "Autorizada",
            Itens = new List<NotaFiscalItem>
            {
                new() { ProdutoId = prod1.Id, Codigo = prod1.Codigo, Descricao = prod1.Descricao, Ncm = prod1.Ncm!, Cfop = "5102", Unidade = prod1.Unidade, Quantidade = 300, ValorUnitario = 135.00m, ValorTotal = 40500.00m, ValorIcms = 4860.00m, ValorPis = 668.25m, ValorCofins = 3078.00m }
            }
        };
        context.NotasFiscais.Add(nfe);

        // 10. Empresa Emitente & Parâmetros
        var empresa = new Empresa
        {
            RazaoSocial = "ERP CORPORATION SISTEMAS INTEGRADOS S/A",
            NomeFantasia = "ERP WEB MATRIX",
            Cnpj = "00.123.456/0001-78",
            Ie = "111.222.333.444",
            Endereco = "Av. das Nações Unidas, 12901 - Brooklin",
            Cidade = "São Paulo",
            Estado = "SP",
            Telefone = "(11) 4002-8922",
            Email = "contato@erpweb.com.br",
            SefazAmbiente = "Homologação"
        };
        context.Empresas.Add(empresa);

        // Audit Log inicial
        context.AuditLogs.Add(new AuditLog
        {
            DataHora = DateTime.UtcNow,
            UsuarioNome = "Sistema",
            Modulo = "Sistema",
            Entidade = "BaseDados",
            Operacao = "SeedInicial",
            Detalhes = "Carga inicial de dados do ERP Web concluída com sucesso."
        });

        await context.SaveChangesAsync();
    }
}
