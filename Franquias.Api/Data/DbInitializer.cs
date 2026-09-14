using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Garante a criação do banco de dados (SQLite)
        await context.Database.EnsureCreatedAsync();

        if (await context.Franqueadoras.AnyAsync())
        {
            return; // Banco já semeado
        }

        // 1. Franqueadora Matriz
        var franqueadora = new Franqueadora
        {
            RazaoSocial = "Rede Café Gourmet Brasil Franchising Ltda",
            NomeFantasia = "Café Gourmet Brasil",
            CNPJ = "12.345.678/0001-90",
            Email = "contato@cafegourmetbrasil.com.br",
            Telefone = "(11) 3050-9000",
            PercentualPadraoRoyalty = 5.0m,
            DataFundacao = new DateTime(2018, 5, 15, 0, 0, 0, DateTimeKind.Utc),
            DataCadastro = DateTime.UtcNow,
            Ativo = true
        };
        await context.Franqueadoras.AddAsync(franqueadora);
        await context.SaveChangesAsync();

        // 2. Responsáveis Franqueados
        var resp1 = new ResponsavelFranqueado
        {
            Nome = "Carlos Eduardo Mendes",
            CPF = "123.456.789-01",
            Email = "carlos.mendes@franquias.com.br",
            Telefone = "(11) 98765-4321",
            DataNascimento = new DateTime(1982, 4, 12, 0, 0, 0, DateTimeKind.Utc),
            DataCadastro = DateTime.UtcNow
        };
        var resp2 = new ResponsavelFranqueado
        {
            Nome = "Juliana Silveira Ramos",
            CPF = "234.567.890-12",
            Email = "juliana.ramos@franquias.com.br",
            Telefone = "(41) 99876-5432",
            DataNascimento = new DateTime(1989, 9, 23, 0, 0, 0, DateTimeKind.Utc),
            DataCadastro = DateTime.UtcNow
        };
        var resp3 = new ResponsavelFranqueado
        {
            Nome = "Roberto Fontes Albuquerque",
            CPF = "345.678.901-23",
            Email = "roberto.fontes@franquias.com.br",
            Telefone = "(21) 97654-3210",
            DataNascimento = new DateTime(1978, 11, 5, 0, 0, 0, DateTimeKind.Utc),
            DataCadastro = DateTime.UtcNow
        };

        await context.Responsaveis.AddRangeAsync(resp1, resp2, resp3);
        await context.SaveChangesAsync();

        // 3. Unidades Franqueadas (2 ativas, 1 inativa para testes de regras de negócio)
        var unidadeSP = new UnidadeFranqueada
        {
            FranqueadoraId = franqueadora.Id,
            ResponsavelId = resp1.Id,
            CodigoUnidade = "FRANQ-SP01",
            Nome = "Café Gourmet - Unidade Moema SP",
            CNPJ = "23.456.789/0001-01",
            Email = "moema@cafegourmetbrasil.com.br",
            Telefone = "(11) 3214-5500",
            Logradouro = "Alameda dos Arapanés",
            Numero = "1240",
            Complemento = "Loja 2",
            Bairro = "Moema",
            Cidade = "São Paulo",
            Estado = "SP",
            CEP = "04524-001",
            DataInicioOperacao = new DateTime(2020, 2, 10, 0, 0, 0, DateTimeKind.Utc),
            Situacao = SituacaoUnidade.Ativa,
            PercentualRoyalty = 5.0m,
            DataCadastro = DateTime.UtcNow
        };

        var unidadePR = new UnidadeFranqueada
        {
            FranqueadoraId = franqueadora.Id,
            ResponsavelId = resp2.Id,
            CodigoUnidade = "FRANQ-PR01",
            Nome = "Café Gourmet - Unidade Batel Curitiba",
            CNPJ = "34.567.890/0001-12",
            Email = "batel@cafegourmetbrasil.com.br",
            Telefone = "(41) 3342-9900",
            Logradouro = "Avenida do Batel",
            Numero = "1850",
            Bairro = "Batel",
            Cidade = "Curitiba",
            Estado = "PR",
            CEP = "80420-090",
            DataInicioOperacao = new DateTime(2021, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            Situacao = SituacaoUnidade.Ativa,
            PercentualRoyalty = 6.0m,
            DataCadastro = DateTime.UtcNow
        };

        var unidadeRJ = new UnidadeFranqueada
        {
            FranqueadoraId = franqueadora.Id,
            ResponsavelId = resp3.Id,
            CodigoUnidade = "FRANQ-RJ01",
            Nome = "Café Gourmet - Unidade Barra RJ",
            CNPJ = "45.678.901/0001-23",
            Email = "barra@cafegourmetbrasil.com.br",
            Telefone = "(21) 2430-8800",
            Logradouro = "Avenida das Américas",
            Numero = "5000",
            Complemento = "Bloco B",
            Bairro = "Barra da Tijuca",
            Cidade = "Rio de Janeiro",
            Estado = "RJ",
            CEP = "22640-102",
            DataInicioOperacao = new DateTime(2022, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            Situacao = SituacaoUnidade.Inativa, // Unidade inativa para validação de bloqueio
            PercentualRoyalty = 5.0m,
            DataCadastro = DateTime.UtcNow
        };

        await context.Unidades.AddRangeAsync(unidadeSP, unidadePR, unidadeRJ);
        await context.SaveChangesAsync();

        // 4. Usuários com senhas criptografadas (BCrypt)
        string hashPadrao = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        string hashGestor = BCrypt.Net.BCrypt.HashPassword("Gestor@123");
        string hashOperador = BCrypt.Net.BCrypt.HashPassword("Operador@123");

        var userAdmin = new Usuario
        {
            Nome = "Wallace Silva (Administrador)",
            Email = "admin@franquias.com.br",
            SenhaHash = hashPadrao,
            Perfil = PerfilUsuario.AdminFranqueadora,
            UnidadeId = null,
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        var userGestorSP = new Usuario
        {
            Nome = "Carlos Mendes (Gestor SP)",
            Email = "gestor.sp@franquias.com.br",
            SenhaHash = hashGestor,
            Perfil = PerfilUsuario.GestorUnidade,
            UnidadeId = unidadeSP.Id,
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        var userOperadorSP = new Usuario
        {
            Nome = "Lucas Santos (Operador SP)",
            Email = "operador.sp@franquias.com.br",
            SenhaHash = hashOperador,
            Perfil = PerfilUsuario.Operador,
            UnidadeId = unidadeSP.Id,
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        var userGestorPR = new Usuario
        {
            Nome = "Juliana Ramos (Gestora PR)",
            Email = "gestor.pr@franquias.com.br",
            SenhaHash = hashGestor,
            Perfil = PerfilUsuario.GestorUnidade,
            UnidadeId = unidadePR.Id,
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        var userOperadorPR = new Usuario
        {
            Nome = "Fernanda Lima (Operadora PR)",
            Email = "operador.pr@franquias.com.br",
            SenhaHash = hashOperador,
            Perfil = PerfilUsuario.Operador,
            UnidadeId = unidadePR.Id,
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        await context.Usuarios.AddRangeAsync(userAdmin, userGestorSP, userOperadorSP, userGestorPR, userOperadorPR);
        await context.SaveChangesAsync();

        // 5. Categorias
        var catCafes = new CategoriaProduto { Nome = "Cafés & Bebidas Especiais", Descricao = "Grãos torrados, cafés moídos e blends exclusivos", Ativo = true };
        var catDoces = new CategoriaProduto { Nome = "Alimentos & Confeitaria", Descricao = "Pães artesanais, croissants e sobremesas", Ativo = true };
        var catAcessorios = new CategoriaProduto { Nome = "Acessórios & Utensílios", Descricao = "Prensas francesas, canecas e moedores", Ativo = true };
        var catServicos = new CategoriaProduto { Nome = "Serviços & Workshops", Descricao = "Treinamentos, degustações guiadas e consultorias", Ativo = true };

        await context.Categorias.AddRangeAsync(catCafes, catDoces, catAcessorios, catServicos);
        await context.SaveChangesAsync();

        // 6. Fornecedores
        var fornGraos = new Fornecedor
        {
            RazaoSocial = "Distribuidora Fazendas do Sul Ltda",
            NomeFantasia = "Grãos do Sul Cafés",
            CNPJ = "11.222.333/0001-44",
            Email = "vendas@graosdosul.com.br",
            Telefone = "(35) 3211-8899",
            Cidade = "Varginha",
            Estado = "MG",
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        var fornConfeitaria = new Fornecedor
        {
            RazaoSocial = "Panificadora & Laticínios Imperial Ltda",
            NomeFantasia = "Imperial Gourmet Bakery",
            CNPJ = "55.666.777/0001-88",
            Email = "pedidos@imperialbakery.com.br",
            Telefone = "(41) 3456-1122",
            Cidade = "Castro",
            Estado = "PR",
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        await context.Fornecedores.AddRangeAsync(fornGraos, fornConfeitaria);
        await context.SaveChangesAsync();

        // 7. Produtos e Serviços Padronizados
        var prodEspresso = new ProdutoServico
        {
            CategoriaId = catCafes.Id,
            FornecedorId = fornGraos.Id,
            CodigoSKU = "CAF-ESP-001",
            Nome = "Café Espresso Blend Nobre 250g",
            Descricao = "Grãos 100% Arábica selecionados com torra média",
            PrecoBase = 38.50m,
            EstoqueMinimoPadrao = 15,
            EServico = false,
            Status = StatusProduto.Ativo,
            DataCadastro = DateTime.UtcNow
        };

        var prodFiltrado = new ProdutoServico
        {
            CategoriaId = catCafes.Id,
            FornecedorId = fornGraos.Id,
            CodigoSKU = "CAF-FIL-002",
            Nome = "Café Reserva Moído 500g",
            Descricao = "Café especial moído com notas de caramelo e chocolate",
            PrecoBase = 52.00m,
            EstoqueMinimoPadrao = 20,
            EServico = false,
            Status = StatusProduto.Ativo,
            DataCadastro = DateTime.UtcNow
        };

        var prodCroissant = new ProdutoServico
        {
            CategoriaId = catDoces.Id,
            FornecedorId = fornConfeitaria.Id,
            CodigoSKU = "ALM-CRO-001",
            Nome = "Croissant Francês Folhado Tradicional",
            Descricao = "Croissant feito com manteiga de primeira linha",
            PrecoBase = 15.00m,
            EstoqueMinimoPadrao = 20,
            EServico = false,
            Status = StatusProduto.Ativo,
            DataCadastro = DateTime.UtcNow
        };

        var prodPaoQueijo = new ProdutoServico
        {
            CategoriaId = catDoces.Id,
            FornecedorId = fornConfeitaria.Id,
            CodigoSKU = "ALM-PDQ-002",
            Nome = "Pão de Queijo da Canastra",
            Descricao = "Pão de queijo tradicional de Minas Gerais",
            PrecoBase = 8.50m,
            EstoqueMinimoPadrao = 30,
            EServico = false,
            Status = StatusProduto.Ativo,
            DataCadastro = DateTime.UtcNow
        };

        var prodWorkshop = new ProdutoServico
        {
            CategoriaId = catServicos.Id,
            FornecedorId = null,
            CodigoSKU = "SRV-WRK-001",
            Nome = "Workshop de Degustação e Métodos de Extração",
            Descricao = "Curso prático de 3 horas com certificado",
            PrecoBase = 180.00m,
            EstoqueMinimoPadrao = 0,
            EServico = true,
            Status = StatusProduto.Ativo,
            DataCadastro = DateTime.UtcNow
        };

        await context.Produtos.AddRangeAsync(prodEspresso, prodFiltrado, prodCroissant, prodPaoQueijo, prodWorkshop);
        await context.SaveChangesAsync();

        // 8. Estoques iniciais e Movimentações
        // SP Moema
        var estEspressoSP = new Estoque
        {
            UnidadeId = unidadeSP.Id,
            ProdutoId = prodEspresso.Id,
            QuantidadeDisponivel = 50,
            QuantidadeMinima = 15,
            UltimaAtualizacao = DateTime.UtcNow
        };

        var estFiltradoSP = new Estoque
        {
            UnidadeId = unidadeSP.Id,
            ProdutoId = prodFiltrado.Id,
            QuantidadeDisponivel = 8, // Estoque Crítico (< 20) para testes do relatório!
            QuantidadeMinima = 20,
            UltimaAtualizacao = DateTime.UtcNow
        };

        var estCroissantSP = new Estoque
        {
            UnidadeId = unidadeSP.Id,
            ProdutoId = prodCroissant.Id,
            QuantidadeDisponivel = 45,
            QuantidadeMinima = 20,
            UltimaAtualizacao = DateTime.UtcNow
        };

        var estPaoQueijoSP = new Estoque
        {
            UnidadeId = unidadeSP.Id,
            ProdutoId = prodPaoQueijo.Id,
            QuantidadeDisponivel = 60,
            QuantidadeMinima = 30,
            UltimaAtualizacao = DateTime.UtcNow
        };

        // Curitiba Batel
        var estEspressoPR = new Estoque
        {
            UnidadeId = unidadePR.Id,
            ProdutoId = prodEspresso.Id,
            QuantidadeDisponivel = 35,
            QuantidadeMinima = 15,
            UltimaAtualizacao = DateTime.UtcNow
        };

        var estFiltradoPR = new Estoque
        {
            UnidadeId = unidadePR.Id,
            ProdutoId = prodFiltrado.Id,
            QuantidadeDisponivel = 30,
            QuantidadeMinima = 20,
            UltimaAtualizacao = DateTime.UtcNow
        };

        var estCroissantPR = new Estoque
        {
            UnidadeId = unidadePR.Id,
            ProdutoId = prodCroissant.Id,
            QuantidadeDisponivel = 10, // Crítico (< 20)
            QuantidadeMinima = 20,
            UltimaAtualizacao = DateTime.UtcNow
        };

        await context.Estoques.AddRangeAsync(estEspressoSP, estFiltradoSP, estCroissantSP, estPaoQueijoSP, estEspressoPR, estFiltradoPR, estCroissantPR);
        await context.SaveChangesAsync();

        // 9. Vendas com Itens e Snapshot de valores
        var vendaSP1 = new Venda
        {
            UnidadeId = unidadeSP.Id,
            UsuarioId = userOperadorSP.Id,
            CodigoVenda = "VND-202609-0001",
            DataHora = DateTime.UtcNow.AddDays(-5),
            ValorTotal = 156.00m,
            Status = StatusVenda.Concluida,
            Observacao = "Consumo local no salão",
            Itens = new List<ItemVenda>
            {
                new() { ProdutoId = prodEspresso.Id, Quantidade = 2, PrecoUnitario = 38.50m, Subtotal = 77.00m },
                new() { ProdutoId = prodCroissant.Id, Quantidade = 3, PrecoUnitario = 15.00m, Subtotal = 45.00m },
                new() { ProdutoId = prodPaoQueijo.Id, Quantidade = 4, PrecoUnitario = 8.50m, Subtotal = 34.00m }
            }
        };

        var vendaSP2 = new Venda
        {
            UnidadeId = unidadeSP.Id,
            UsuarioId = userOperadorSP.Id,
            CodigoVenda = "VND-202609-0002",
            DataHora = DateTime.UtcNow.AddDays(-2),
            ValorTotal = 284.00m,
            Status = StatusVenda.Concluida,
            Observacao = "Inscrição de workshop e produtos",
            Itens = new List<ItemVenda>
            {
                new() { ProdutoId = prodWorkshop.Id, Quantidade = 1, PrecoUnitario = 180.00m, Subtotal = 180.00m },
                new() { ProdutoId = prodFiltrado.Id, Quantidade = 2, PrecoUnitario = 52.00m, Subtotal = 104.00m }
            }
        };

        var vendaPR1 = new Venda
        {
            UnidadeId = unidadePR.Id,
            UsuarioId = userOperadorPR.Id,
            CodigoVenda = "VND-202609-0003",
            DataHora = DateTime.UtcNow.AddDays(-3),
            ValorTotal = 115.50m,
            Status = StatusVenda.Concluida,
            Observacao = "Balcão delivery",
            Itens = new List<ItemVenda>
            {
                new() { ProdutoId = prodEspresso.Id, Quantidade = 3, PrecoUnitario = 38.50m, Subtotal = 115.50m }
            }
        };

        await context.Vendas.AddRangeAsync(vendaSP1, vendaSP2, vendaPR1);
        await context.SaveChangesAsync();

        // 10. Royalties apurados
        var royaltyAgosto = new Royalty
        {
            UnidadeId = unidadeSP.Id,
            MesReferencia = 8,
            AnoReferencia = 2026,
            FaturamentoBase = 12500.00m,
            PercentualCobrado = 5.0m,
            ValorRoyalty = 625.00m,
            Status = StatusRoyalty.Pago,
            DataVencimento = new DateTime(2026, 9, 10, 0, 0, 0, DateTimeKind.Utc),
            DataPagamento = new DateTime(2026, 9, 8, 14, 30, 0, DateTimeKind.Utc),
            DataApuracao = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            Observacao = "Quitado dentro do prazo"
        };

        var royaltySetembro = new Royalty
        {
            UnidadeId = unidadeSP.Id,
            MesReferencia = 9,
            AnoReferencia = 2026,
            FaturamentoBase = 440.00m,
            PercentualCobrado = 5.0m,
            ValorRoyalty = 22.00m,
            Status = StatusRoyalty.Pendente,
            DataVencimento = new DateTime(2026, 10, 10, 0, 0, 0, DateTimeKind.Utc),
            DataApuracao = DateTime.UtcNow,
            Observacao = "Apurado via sistema"
        };

        await context.Royalties.AddRangeAsync(royaltyAgosto, royaltySetembro);
        await context.SaveChangesAsync();

        // 11. Chamados de Suporte
        var chamado1 = new ChamadoSuporte
        {
            UnidadeId = unidadeSP.Id,
            UsuarioAberturaId = userGestorSP.Id,
            Titulo = "Dúvida sobre apuração de royalties de setembro",
            Descricao = "Gostaria de confirmar se as vendas de workshops entram no cálculo padrão de royalties.",
            Categoria = CategoriaChamado.Financeiro,
            Prioridade = PrioridadeChamado.Media,
            Status = StatusChamado.Resolvido,
            DataAbertura = DateTime.UtcNow.AddDays(-6),
            DataAtualizacao = DateTime.UtcNow.AddDays(-4),
            DataEncerramento = DateTime.UtcNow.AddDays(-4),
            RespostaSolucao = "Sim, conforme o contrato de franquia cláusula 7, todos os serviços e produtos compõem o faturamento bruto."
        };

        var chamado2 = new ChamadoSuporte
        {
            UnidadeId = unidadePR.Id,
            UsuarioAberturaId = userGestorPR.Id,
            Titulo = "Solicitação de novos displays de marketing da campanha de primavera",
            Descricao = "Precisamos do material promocional físico para expor o novo café reserva.",
            Categoria = CategoriaChamado.Marketing,
            Prioridade = PrioridadeChamado.Alta,
            Status = StatusChamado.EmAndamento,
            DataAbertura = DateTime.UtcNow.AddDays(-1),
            DataAtualizacao = DateTime.UtcNow,
            RespostaSolucao = "Material despachado via transportadora no dia 13/09 com rastreio BR2026."
        };

        await context.Chamados.AddRangeAsync(chamado1, chamado2);
        await context.SaveChangesAsync();
    }
}
