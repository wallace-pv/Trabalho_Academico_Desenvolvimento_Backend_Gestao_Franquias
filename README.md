# Sistema de Gestão de Franquias — API REST

**Trabalho Acadêmico da Disciplina:** Desenvolvimento Web Back-end  
**Instituição:** Centro Universitário Internacional UNINTER (Ano 2026)  
**Estudante:** WALLACE F G SILVA — **RU:** 5146520  
**Professor Orientador:** Prof. Rodrigo da S. do Nascimento  
**Repositório Oficial no GitHub:** [https://github.com/wallace-pv/Trabalho_Academico_Desenvolvimento_Backend_Gestao_Franquias](https://github.com/wallace-pv/Trabalho_Academico_Desenvolvimento_Backend_Gestao_Franquias)

---

> ### 🎓 Guia Rápido de Avaliação para o Professor:
> 1. Clone o repositório ou abra a pasta do projeto:
>    ```bash
>    git clone https://github.com/wallace-pv/Trabalho_Academico_Desenvolvimento_Backend_Gestao_Franquias.git
>    cd Trabalho_Academico_Desenvolvimento_Backend_Gestao_Franquias
>    ```
> 2. Execute a API com um único comando (o banco SQLite e os dados de exemplo são criados automaticamente):
>    ```bash
>    dotnet run --project Franquias.Api
>    ```
> 3. Abra o navegador em: 👉 **[http://localhost:5086/](http://localhost:5086/)**
> 4. Faça login no Swagger em `POST /api/Auth/login` com o usuário **`admin@franquias.com.br`** / senha **`Admin@123`**, copie o token e clique no botão verde **Authorize** (`Bearer {token}`). Todos os relatórios e endpoints estarão prontos para teste imediato!

---

## 📌 Visão Geral do Projeto
O **Sistema de Gestão de Franquias** é uma API REST desenvolvida em **C#** e **ASP.NET Core Web API**, projetada para centralizar a operação entre a franqueadora (matriz) e suas unidades franqueadas. A solução resolve o problema de descentralização corporativa, eliminando o uso de planilhas isoladas e integrando em um único back-end:

- Autenticação e autorização baseada em papéis (RBAC com JWT).
- Gestão de unidades franqueadas e responsáveis legais.
- Catálogo padronizado de produtos e serviços.
- Gestão de fornecedores homologados.
- Controle de estoque por unidade com impedimento estrito de saldo negativo.
- Registro de vendas com baixa atômica de estoque e cálculo automático de totais.
- Apuração e liquidação financeira de taxas e royalties por competência.
- Abertura, tramitação e encerramento de chamados de suporte técnico/operacional.
- Geração de relatórios gerenciais e dashboards analíticos com LINQ.

---

## 🛠️ Tecnologias Utilizadas
- **Linguagem:** C# 12 (.NET 8.0 SDK)
- **Framework Web:** ASP.NET Core Web API
- **ORM / Persistência:** Entity Framework Core 8.0
- **Banco de Dados Relacional:** SQLite (com suporte nativo a transações ACID, Migrations e zero configuração externa)
- **Segurança:** Autenticação JWT (*JSON Web Tokens*) com `Microsoft.AspNetCore.Authentication.JwtBearer`
- **Criptografia de Senhas:** BCrypt.Net-Next (algoritmo PBKDF2/BCrypt com Salt)
- **Documentação de API:** OpenAPI / Swagger UI interativo
- **Testes de Integração:** Arquivo nativo `api-tests.http` para VS Code REST Client, Visual Studio e Bruno/Postman

---

## 🏛️ Arquitetura e Organização do Código

A solução foi estruturada seguindo o padrão de separação de responsabilidades em camadas:

```
Trabalho_Academico_Desenvolvimento_Backend_Gestao_Franquias/
├── Franquias.Api/
│   ├── Controllers/             # Endpoints REST com anotações e códigos HTTP padronizados
│   │   ├── AuthController.cs
│   │   ├── FranqueadorasController.cs
│   │   ├── UnidadesController.cs
│   │   ├── CategoriasController.cs
│   │   ├── ProdutosController.cs
│   │   ├── FornecedoresController.cs
│   │   ├── EstoquesController.cs
│   │   ├── VendasController.cs
│   │   ├── RoyaltiesController.cs
│   │   ├── ChamadosController.cs
│   │   └── RelatoriosController.cs
│   ├── Models/                  # Entidades do domínio relacional e Enums
│   │   ├── Enums/
│   │   ├── Franqueadora.cs
│   │   ├── ResponsavelFranqueado.cs
│   │   ├── UnidadeFranqueada.cs
│   │   ├── Usuario.cs
│   │   ├── CategoriaProduto.cs
│   │   ├── ProdutoServico.cs
│   │   ├── Fornecedor.cs
│   │   ├── Estoque.cs
│   │   ├── MovimentacaoEstoque.cs
│   │   ├── Venda.cs
│   │   ├── ItemVenda.cs
│   │   ├── Royalty.cs
│   │   └── ChamadoSuporte.cs
│   ├── DTOs/                    # Contratos de transferência de dados (Request / Response)
│   ├── Services/                # Camada de lógica de negócio e validações de regras
│   │   ├── Interfaces/
│   │   └── Implementations/
│   ├── Repositories/            # Abstração de persistência e consultas LINQ
│   │   ├── Interfaces/
│   │   └── Implementations/
│   ├── Data/                    # DbContext EF Core, Mapeamento Fluent API e Seed
│   │   ├── AppDbContext.cs
│   │   └── DbInitializer.cs
│   ├── Migrations/              # Histórico versionado de Migrations do banco
│   ├── Configurations/          # DI, JWT, Swagger e Middleware global de exceções
│   ├── Program.cs               # Configuração do pipeline HTTP e inicialização
│   └── appsettings.json
├── api-tests.http               # Coleção de testes pronta para execução
├── README.md                    # Manual do projeto
└── docs/                        # Relatório acadêmico (.docx e .md)
```

---

## 🔑 Perfis de Acesso e Credenciais de Teste

O sistema já é inicializado automaticamente com usuários e dados de teste pré-cadastrados para validação imediata do avaliador:

| Perfil | E-mail | Senha | Descrição de Acesso |
|---|---|---|---|
| **AdminFranqueadora** | `admin@franquias.com.br` | `Admin@123` | Acesso total irrestrito a todas as unidades, cadastros mestres, royalties e relatórios globais |
| **GestorUnidade (SP)** | `gestor.sp@franquias.com.br` | `Gestor@123` | Gerencia a unidade Moema SP, estoque, vendas, chamados e seus operadores |
| **Operador (SP)** | `operador.sp@franquias.com.br` | `Operador@123` | Operador de PDV da unidade Moema SP (registro de vendas e movimentações) |
| **GestorUnidade (PR)** | `gestor.pr@franquias.com.br` | `Gestor@123` | Gestor da unidade Batel Curitiba |
| **Operador (PR)** | `operador.pr@franquias.com.br` | `Operador@123` | Operador de PDV da unidade Curitiba |

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
- .NET 8.0 SDK instalado ([Download oficial](https://dotnet.microsoft.com/download/dotnet/8.0))

### Passo a Passo:
1. Abra um terminal na pasta raiz do projeto:
   ```bash
   cd Trabalho_Academico_Desenvolvimento_Backend_Gestao_Franquias
   ```
2. Restaure as dependências e compile a aplicação:
   ```bash
   dotnet restore
   dotnet build
   ```
3. Execute a API:
   ```bash
   dotnet run --project Franquias.Api
   ```
4. A API será iniciada. O Swagger UI estará disponível diretamente no navegador em:
   👉 **http://localhost:5086/** (ou na porta indicada no console)

> **Nota:** Ao iniciar pela primeira vez, o `DbInitializer` cria automaticamente o banco de dados `Franquias.db` e insere o conjunto completo de dados de exemplo (Franqueadora, 3 Unidades, Usuários, Produtos, Estoques, Vendas e Chamados). Não é necessário rodar nenhum comando manual de banco de dados.

---

## 🧪 Como Testar os Endpoints

### Opção 1: Via Swagger UI (Interativo no Navegador)
1. Acesse `http://localhost:5086/`.
2. Expanda o endpoint `POST /api/auth/login` e clique em **Try it out**.
3. Use o login do administrador (`admin@franquias.com.br` / `Admin@123`) e clique em **Execute**.
4. Copie o valor do campo `"token"` retornado.
5. Role até o topo da página do Swagger e clique no botão verde **Authorize**.
6. Digite `Bearer {seu_token}` e confirme.
7. Todos os endpoints autenticados estarão liberados para teste!

### Opção 2: Via Arquivo `api-tests.http`
- Abra o arquivo `api-tests.http` no **VS Code** (com a extensão *REST Client*) ou no **Visual Studio**.
- Clique em **Send Request** logo acima de cada requisição. Os tokens são salvos em variáveis automaticamente.

---

## 📋 Regras de Negócio Implementadas

1. **Unicidade de CNPJ de Unidades:** O sistema não permite cadastrar duas unidades com o mesmo CNPJ (validação de serviço + constraint `Index Unique` no banco relacional).
2. **Unicidade de E-mail de Usuários:** Validação contra e-mails duplicados para garantir integridade na autenticação.
3. **Bloqueio de Vendas em Unidade Inativa:** Unidades com situação `Inativa` não podem registrar novas vendas (retorna HTTP 400 Bad Request explicativo).
4. **Validação de Venda com Itens:** Uma venda deve pertencer a uma única unidade e conter no mínimo 1 item com quantidade estritamente positiva.
5. **Impedimento de Saldo Negativo de Estoque:** Nenhuma movimentação de saída manual ou venda pode resultar em saldo de estoque menor que zero. Caso o saldo seja insuficiente, a transação é cancelada e um erro descritivo é retornado.
6. **Atualização Atômica de Estoque na Venda:** Ao confirmar a venda, o estoque da unidade é decrementado automaticamente de forma transacional.
7. **Cálculo Automático de Preços e Subtotais:** A venda grava o snapshot do preço unitário do produto no momento da transação e calcula os subtotais e valor total automaticamente.
8. **Apuração de Royalties por Período:** O valor dos royalties devidos é calculado aplicando-se a alíquota contratual da unidade (ex: 5% a 6%) sobre o faturamento total das vendas concluídas na competência (mês/ano).
9. **Controle de Acesso Baseado em Perfis (RBAC):** Endpoints administrativos (como apuração de royalties, inativação de unidades e catálogo) são restritos ao perfil `AdminFranqueadora`, enquanto operações diárias são restritas a operadores e gestores.
10. **Tratamento Global de Exceções:** Middleware centralizado que converte exceções de negócio em retornos HTTP coerentes (`400 Bad Request`, `401 Unauthorized`, `403 Forbidden`, `404 Not Found`, `500 Internal Server Error`).

---

## 📊 Relatórios e Indicadores Gerenciais Disponíveis
- `GET /api/relatorios/faturamento`: Faturamento consolidado, quantidade de vendas e ticket médio de uma unidade por intervalo de datas.
- `GET /api/relatorios/ranking-unidades`: Ranking comparativo das franquias ordenadas por faturamento decrescente.
- `GET /api/relatorios/royalties`: Total de royalties gerados, pagos, pendentes e atrasados.
- `GET /api/relatorios/produtos-mais-vendidos`: Top produtos e serviços com maior volume de vendas e faturamento gerado.
- `GET /api/relatorios/estoque-critico`: Relação em tempo real de produtos cujo saldo disponível atingiu ou ficou abaixo da quantidade mínima.
- `GET /api/relatorios/chamados-status`: Contagem e distribuição de chamados por situação operacional.
- `GET /api/relatorios/dashboard`: Painel executivo consolidado da rede de franquias.

---

## 📄 Relatório Acadêmico Entregue
O relatório acadêmico completo exigido pela UNINTER encontra-se disponível na pasta `docs/`:
- `docs/Relatorio_Academico_Gestao_Franquias.docx` (Documento Word oficial para submissão no Univirtus)
- `docs/Relatorio_Academico_Gestao_Franquias.md` (Versão em Markdown para leitura no repositório)
