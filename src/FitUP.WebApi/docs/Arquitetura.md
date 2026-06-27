# FitUP — Arquitetura

> **Propósito:** referência técnica canônica. Toda decisão de tecnologia, padrão de código e estrutura de projeto está documentada aqui. Uma IA que ler este arquivo nunca deve sugerir stack incompatível.

---

## 1. Stack Tecnológica

### 1.1 Back-end

| Camada | Tecnologia | Versão | Observação |
|---|---|---|---|
| Runtime | .NET | **10.0** | Target framework: `net10.0` |
| Framework web | ASP.NET Core | 10.* | Minimal hosting model (`WebApplication.CreateBuilder`) |
| API doc | Swashbuckle | 7.* | Swagger UI em `/swagger` (só Development) |
| Autenticação | JwtBearer | 10.* | HMAC-SHA256, chave simétrica |
| Hash senhas | BCrypt.Net-Next | 4.* | Cost 10-12 (padrão da lib) |
| Acesso a dados | **Microsoft.Data.SqlClient** | 6.* | **ADO.NET puro. Sem ORM. Sem EF Core.** |
| Banco | SQL Server Express | **2025** | Windows Authentication |
| Linguagem | C# | 14 | `ImplicitUsings: enable`, `Nullable: enable` |

### 1.2 Front-end

| Camada | Tecnologia | Versão | Observação |
|---|---|---|---|
| Runtime | .NET | **10.0** | Blazor WebAssembly |
| Hosting | Blazor WASM | 10.* | **Não é Blazor Server. Não é Blazor Hybrid.** |
| Componentes | **MudBlazor** | 9.* | Material Design. Única biblioteca de UI. |
| Estilização | CSS3 puro | — | Sem pré-processador (SASS/LESS). Sem Tailwind. |
| HTTP | `System.Net.Http.HttpClient` | built-in | `System.Net.Http.Json` para JSON |
| Linguagem | C# | 14 | `ImplicitUsings: enable`, `Nullable: enable` |

### 1.3 O que NÃO usamos (e nunca deve ser sugerido)

| Tecnologia | Motivo |
|---|---|
| Entity Framework Core | Projeto usa ADO.NET puro. Sem DbContext, sem migrations. |
| Dapper | Usa Microsoft.Data.SqlClient direto. Sem micro-ORM. |
| PostgreSQL / MySQL / SQLite | SQL Server Express 2025. Não é multi-banco. |
| Blazor Server / Hybrid | Blazor WebAssembly puro. Renderiza no navegador. |
| Bootstrap / Tailwind | MudBlazor como único kit de componentes UI. |
| SASS / LESS | CSS3 puro. |
| Node.js / npm / webpack | Ecossistema .NET puro. Zero dependências Node. |
| AutoMapper | Mapeamento manual de DTOs nos Services. |
| MediatR / CQRS | Padrão Controller → Service simples. |
| FluentValidation | Validação via DataAnnotations no Model (quando existir). |
| Docker (atualmente) | Não configurado. Rodar com `dotnet run` direto. |

---

## 2. Estrutura de Diretórios e Responsabilidades

```
FitUP/                                     ← Raiz do repositório
│
├── FitUP.slnx                             ← Solution (referencia só o front-end atualmente)
│
├── FitUP/                                 ← PROJETO FRONT-END (Blazor WASM)
│   ├── Program.cs                         ← Entry point. Registra MudBlazor, HttpClient (base: localhost:5000), AuthService
│   ├── App.razor                          ← Router + fallback 404
│   ├── _Imports.razor                     ← Global usings (MudBlazor, System.Net.Http, etc.)
│   ├── FitUP.csproj                       ← net10.0, MudBlazor 9.*, Blazor WASM 10.*
│   │
│   ├── Pages/                             ← Componentes Razor (páginas) — 12 arquivos
│   │   ├── Home.razor                     ← Landing page. Cards de treino (Upper/Lower, Fullbody, PPL, ABCD, Alimentação)
│   │   ├── Login.razor                    ← Form de login. Integrado com AuthService → API real.
│   │   ├── Cadastro.razor                 ← Form de cadastro. Integrado com AuthService → API real. ⚠️ Tem conflito de merge.
│   │   ├── DicasTreino.razor              ← Conteúdo estático: métodos de treino com MudDialog
│   │   ├── GanhoMaximo.razor              ← Conteúdo estático: guias alimentares com MudStepper
│   │   ├── CalculadoraBio.razor           ← Calculadora local (sem API). Inputs + fórmulas no cliente.
│   │   ├── MonteTreino.razor              ← Placeholder "Em construção"
│   │   ├── Indicacoes.razor               ← Links externos para academias
│   │   ├── NotFound.razor                 ← Página 404
│   │   ├── MensagemCustomizada.razor      ← Diálogo reutilizável
│   │   └── Teste.razor                    ← Página de teste
│   │
│   ├── Layout/                            ← Layout components
│   │   ├── MainLayout.razor               ← Layout principal. AppBar, Drawer, Footer, temas Dark/Light, background.
│   │   ├── MainLayout.razor.css           ← Estilos do layout (legado do template Blazor)
│   │   └── NavMenu.razor                  ← Menu de navegação lateral (Drawer)
│   │
│   ├── Services/                          ← Serviços de comunicação com a API
│   │   └── AuthService.cs                 ← Único serviço HTTP. Contém DTOs próprios (LoginRequest, RegistroRequest, AuthResponse)
│   │                                       ← ⚠️ Esses DTOs DUPLICAM os do back-end. Compartilhar? Manter assim?
│   │
│   └── wwwroot/                           ← Assets estáticos
│       ├── index.html                     ← Entry point SPA. Referencia CSS e JS do framework.
│       ├── css/app.css                    ← Estilos globais (loading, error boundary, fontes)
│       ├── img/                           ← Imagens da home e treinos (Home.png, H.PPL.png, etc.)
│       ├── img-dt/                        ← Imagens de exercícios (supinoreto.png, stiff.png, etc.)
│       ├── img-gm/                        ← Imagens de planos alimentares (cafe1.png, almoco2.png, etc.)
│       └── sample-data/weather.json       ← Dados de exemplo (legado do template)
│
├── src/FitUP.WebApi/                      ← PROJETO BACK-END (ASP.NET Core Web API)
│   ├── Program.cs                         ← Host builder. JWT, Swagger, CORS, DI (6 serviços Scoped). Middleware pipeline.
│   ├── appsettings.json                   ← ConnectionStrings:DefaultConnection + Jwt (Key, Issuer, Audience, ExpiresInMinutes)
│   ├── FitUP.WebApi.csproj                ← net10.0, SqlClient 6.*, BCrypt 4.*, JwtBearer 10.*, Swashbuckle 7.*
│   │
│   ├── Controllers/                       ← 5 controllers, 22 endpoints
│   │   ├── AuthController.cs              ← POST login, registrar, refresh-token (público, sem [Authorize])
│   │   ├── UsuarioController.cs           ← GET/PUT/DELETE me, PUT alterar-senha ([Authorize])
│   │   ├── PlanoTreinoController.cs       ← CRUD /api/planos-treino ([Authorize])
│   │   ├── PlanoAlimentarController.cs    ← CRUD /api/planos-alimentares ([Authorize])
│   │   └── RegistroBioimpedanciaController.cs ← CRUD /api/bioimpedancia ([Authorize])
│   │
│   ├── Service/                           ← 6 interfaces + 6 implementações
│   │   ├── ITokenService.cs / TokenService.cs         ← Geração de JWT (HMAC-SHA256) + Refresh Token (RNG 64 bytes)
│   │   ├── IAuthService.cs / AuthService.cs           ← Login, Registro, RefreshToken (ADO.NET + BCrypt)
│   │   ├── IUsuarioService.cs / UsuarioService.cs     ← CRUD de perfil + alterar senha + soft delete
│   │   ├── IPlanoTreinoService.cs / PlanoTreinoService.cs   ← CRUD de planos com JOIN dias/exercícios
│   │   ├── IPlanoAlimentarService.cs / PlanoAlimentarService.cs ← CRUD de planos com JOIN refeições/alimentos
│   │   └── IRegistroBioimpedanciaService.cs / RegistroBioimpedanciaService.cs ← CRUD de registros
│   │
│   ├── DTOs/                              ← 13 objetos de transferência (entrada/saída)
│   │   ├── LoginRequest.cs                ← { email, senha }
│   │   ├── RegistroRequest.cs             ← { nome, sobrenome, email, senha, telefone?, cpf?, dataNascimento? }
│   │   ├── AuthResponse.cs                ← { usuarioId, nome, email, token, refreshToken, expiraEm }
│   │   ├── RefreshTokenRequest.cs          ← { refreshToken }
│   │   ├── UsuarioDto.cs / UsuarioUpdateRequest.cs / AlterarSenhaRequest.cs
│   │   ├── PlanoTreinoDto.cs / PlanoTreinoRequest.cs    ← Dto inclui DiaTreinoDto[] e ExercicioDto[]
│   │   ├── PlanoAlimentarDto.cs / PlanoAlimentarRequest.cs ← Dto inclui RefeicaoDto[] e AlimentoDto[]
│   │   └── RegistroBioimpedanciaDto.cs / RegistroBioimpedanciaRequest.cs
│   │
│   ├── Models/                            ← 8 entidades POCO (sem navegação, sem data annotations)
│   │   ├── Usuario.cs                     ← Id, Nome, Sobrenome, Email, SenhaHash, RefreshToken, Ativo, etc.
│   │   ├── PlanoTreino.cs / DiaTreino.cs / Exercicio.cs
│   │   ├── PlanoAlimentar.cs / Refeicao.cs / Alimento.cs
│   │   └── RegistroBioimpedancia.cs
│   │
│   ├── Infrastructure/
│   │   └── script.sql                     ← Schema completo: CREATE DATABASE + 8 tabelas + FKs CASCADE + 7 índices
│   │
│   ├── docs/                              ← Documentação do projeto
│   │   └── visao.md / Arquitetura.md
│   │
│   └── Properties/
│       └── launchSettings.json            ← Portas: HTTP 5000, HTTPS 5001. Launch URL: swagger.
│
└── README.md                              ← Visão geral do projeto, time, tecnologias
```

---

## 3. Comunicação entre Módulos

### 3.1 Diagrama de fluxo

```
┌─────────────────────────────────────────┐
│  Navegador                               │
│  └── Blazor WASM (MudBlazor UI)         │
│      └── HttpClient                     │
│          └── BaseAddress: localhost:5000 │
└──────────────┬──────────────────────────┘
               │  HTTP/1.1
               │  Header: Content-Type: application/json
               │  Header: Authorization: Bearer {jwt}  (rotas protegidas)
               │  Body: JSON (camelCase)
               │
               ▼
┌─────────────────────────────────────────┐
│  ASP.NET Core Middleware Pipeline        │
│  1. UseSwagger (só Development)          │
│  2. UseHttpsRedirection                  │
│  3. UseCors("AllowFrontend")             │  ← origens: localhost:5059, localhost:7211
│  4. UseAuthentication  ← valida JWT      │
│  5. UseAuthorization    ← verifica [Auth] │
│  6. MapControllers      ← roteia rotas   │
└──────────────┬──────────────────────────┘
               │  Controller extrai UsuarioId do claim:
               │  Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
               │  Chama: _service.MetodoAsync(UsuarioIdLogado, request)
               │
               ▼
┌─────────────────────────────────────────┐
│  Service (Scoped)                        │
│  ├── new SqlConnection(_connectionString)│
│  ├── await connection.OpenAsync()        │
│  ├── new SqlCommand(sql, connection)      │
│  ├── command.Parameters.AddWithValue()   │
│  ├── await command.ExecuteReaderAsync()  │
│  ├── Mapeamento manual → DTO             │
│  └── return DTO                          │
└──────────────┬──────────────────────────┘
               │  TDS (Tabular Data Stream)
               │  Windows Authentication
               │
               ▼
┌─────────────────────────────────────────┐
│  SQL Server Express 2025                 │
│  Database: FitUP                         │
│  8 tabelas + 7 índices                   │
└─────────────────────────────────────────┘
```

### 3.2 Padrões de comunicação

| Entre | Mecanismo | Detalhe |
|---|---|---|
| Front-end → Back-end | HTTP REST JSON | `HttpClient` com `BaseAddress = "http://localhost:5000"`. Chamadas com `PostAsJsonAsync` / `GetFromJsonAsync`. |
| Back-end → Banco | ADO.NET | `Microsoft.Data.SqlClient`. Conexão criada por método (`new SqlConnection`). 100% parametrizado com `SqlParameter`. |
| Auth (toda requisição) | JWT Bearer | Token no header `Authorization: Bearer {jwt}`. Back-end valida assinatura HMAC-SHA256, issuer, audience, expiry. |
| CORS | Middleware | Política `AllowFrontend`: só aceita requests de `localhost:5059` e `localhost:7211`. |

### 3.3 Formato de dados

- **JSON camelCase** em todas as requisições e respostas (padrão ASP.NET Core com `System.Text.Json`).
- **Datas:** ISO 8601 UTC (`DateTime` no C#, `DATETIME2` no SQL).
- **Guids:** string formato `"xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"`.
- **Nullable:** campos opcionais serializam como `null` ou são omitidos.

---

## 4. Banco de Dados

### 4.1 Tecnologia

| Item | Valor |
|---|---|
| SGBD | SQL Server Express 2025 |
| Autenticação | Windows Authentication (`Trusted_Connection=True`) |
| Connection string | `Server=.\SQLEXPRESS;Database=FitUP;Trusted_Connection=True;TrustServerCertificate=True;` |
| Schema | `src/FitUP.WebApi/Infrastructure/script.sql` — fonte única de verdade |
| Migrations | **Não usa.** Script SQL manual. Alterações de schema: editar script.sql + aplicar no banco. |

### 4.2 Modelo de dados

```
Usuario (1) ──< (N) PlanoTreino (1) ──< (N) DiaTreino (1) ──< (N) Exercicio
Usuario (1) ──< (N) PlanoAlimentar (1) ──< (N) Refeicao (1) ──< (N) Alimento
Usuario (1) ──< (N) RegistroBioimpedancia
```

**8 tabelas, 7 índices non-clustered, 7 foreign keys com ON DELETE CASCADE.**

### 4.3 Convenções

| Regra | Detalhe |
|---|---|
| PK | `UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID()` |
| FK | `ON DELETE CASCADE` (remoção em cascata até as folhas) |
| Datas | `DATETIME2`. Timestamps de criação com `DEFAULT GETUTCDATE()`. Sempre UTC. |
| Booleanos | `BIT`, `DEFAULT 0` ou `DEFAULT 1` |
| Dinheiro/Medidas | `FLOAT` (não `DECIMAL`/`MONEY`). Dobro no C#: `double`. |
| Texto curto | `NVARCHAR(n)` — sempre Unicode. |
| Texto longo | `NVARCHAR(MAX)` — usado para SenhaHash. |
| Nulos | Explicitamente `NULL` ou `NOT NULL`. Sem defaults implícitos. |
| Enums | Armazenados como `INT`. Sem tabela de enum. Valores definidos no visao.md. |

### 4.4 Índices

```sql
CREATE INDEX IX_PlanoTreino_UsuarioId ON PlanoTreino(UsuarioId);
CREATE INDEX IX_DiaTreino_PlanoTreinoId ON DiaTreino(PlanoTreinoId);
CREATE INDEX IX_Exercicio_DiaTreinoId ON Exercicio(DiaTreinoId);
CREATE INDEX IX_PlanoAlimentar_UsuarioId ON PlanoAlimentar(UsuarioId);
CREATE INDEX IX_Refeicao_PlanoAlimentarId ON Refeicao(PlanoAlimentarId);
CREATE INDEX IX_Alimento_RefeicaoId ON Alimento(RefeicaoId);
CREATE INDEX IX_RegistroBioimpedancia_UsuarioId ON RegistroBioimpedancia(UsuarioId);
```

---

## 5. Integrações Externas

**Nenhuma.** O sistema é autocontido:

- Sem APIs de terceiros (CEP, pagamento, notificação).
- Sem serviços cloud (Azure, AWS, GCP).
- Sem provedor de email/SMS.
- Sem CDN (todos assets são servidos localmente).
- Sem autenticação externa (Google, Facebook, etc.) — apenas JWT próprio.

Links externos no front-end são puramente informativos (GitHub, LinkedIn, Instagram, linktr.ee) — não são integrações.

---

## 6. Decisões Técnicas que Impactam o Desenvolvimento

### 6.1 ADO.NET puro (sem ORM)

**Decisão:** Não usar Entity Framework Core nem Dapper. Acesso a dados via `Microsoft.Data.SqlClient` com SQL escrita à mão e mapeamento manual de `SqlDataReader` para DTOs.

**Impactos:**
- Toda query é uma string SQL explícita. Não há LINQ, IQueryable, ou lazy loading.
- Não há migrations. Alterações de schema são feitas direto no script.sql.
- Mapeamento de resultados é manual e verboso (`reader.GetGuid(reader.GetOrdinal("Id"))`).
- O método `AddWithValue` é usado atualmente — **atenção**: `AddWithValue` infere o tipo e pode causar planos de execução ruins. Ideal migrar para `Add(nome, SqlDbType, tamanho).Value = valor`.
- Cada método cria sua própria `SqlConnection`. Não há pool gerenciado pela aplicação (embora o ADO.NET tenha pool interno por connection string).

### 6.2 UNIQUEIDENTIFIER como PK

**Decisão:** Todas as chaves primárias são `Guid` (UNIQUEIDENTIFIER no SQL Server), geradas com `NEWSEQUENTIALID()` no banco ou `Guid.NewGuid()` no código C#.

**Impactos:**
- URLs expõem Guids: `/api/planos-treino/3fa85f64-5717-4562-b3fc-2c963f66afa6`.
- Não sequencial, difícil de adivinhar (segurança por obscuridade parcial).
- Controllers usam constraint `{id:guid}` nas rotas.
- Não usar `int` auto-incremento em hipótese alguma.

### 6.3 Models POCO sem navegação

**Decisão:** As classes em `Models/` não têm propriedades de navegação, data annotations, nem atributos de ORM. São objetos puros.

**Impactos:**
- Não há `.Include()` ou carregamento de entidades relacionadas — tudo é feito com JOINs manuais nos Services.
- Models nunca são retornados diretamente pela API. Sempre passam por DTOs.
- Nomes de propriedade em PascalCase no C#, serializados como camelCase no JSON.

### 6.4 DTOs duplicados no front-end

**Decisão:** O front-end (`FitUP/Services/AuthService.cs`) define seus próprios DTOs (`LoginRequest`, `RegistroRequest`, `AuthResponse`) com `JsonPropertyName` attributes. Não compartilha os DTOs do back-end.

**Impactos:**
- Duas fontes de verdade para os mesmos contratos. Alterar um DTO no back-end exige alterar o correspondente no front-end.
- Os DTOs do front-end usam `JsonPropertyName` explícito; os do back-end dependem da serialização padrão (camelCase).

### 6.5 Sem camada Repository

**Decisão:** Services chamam SQL direto. Não há interface `IRepository<T>` ou classe base `Repository`.

**Impactos:**
- SQL está acoplada ao Service. Para trocar de banco, reescrever cada Service.
- Simplicidade: menos camadas, menos arquivos, menos injeção de dependência.

### 6.6 Autenticação via JWT (stateless)

**Decisão:** Tokens JWT assinados com HMAC-SHA256, chave simétrica definida em `appsettings.json`. Refresh token persistido no banco (rotativo).

**Impactos:**
- Chave JWT hardcoded no appsettings.json — **não é seguro para produção**. Deve ir para secrets ou variável de ambiente.
- Claims disponíveis: `NameIdentifier` (Id), `Name` (Nome), `Email`, `GivenName` (Nome Sobrenome).
- Controller extrai `UsuarioId` do claim, nunca de parâmetro da rota/body. Isso impede que um usuário acesse dados de outro.
- `ClockSkew = TimeSpan.Zero` — token expira exatamente no prazo, sem tolerância.

### 6.7 Soft delete para Usuario

**Decisão:** `Usuario.Ativo = 0` ao "deletar" conta. Dados permanecem no banco. Planos e registros associados são preservados.

**Impactos:**
- Queries de login verificam `Ativo = 1`.
- Não há endpoint para reativar conta.
- Hard delete (CASCADE) só para planos, dias, exercícios, refeições, alimentos, registros — acionado pelo usuário explicitamente.

### 6.8 MudBlazor como único kit de UI

**Decisão:** Todos os componentes de interface vêm do MudBlazor. Paleta de cores customizada (coral `#ff7f50` como cor primária).

**Impactos:**
- Não usar Bootstrap, Tailwind, ou HTML raw para componentes de formulário/tabela/diálogo.
- Tema escuro por padrão (`_isDarkMode = true`). Usuário alterna via botão.
- Todos os estilos inline ou em `<style>` tags nos próprios componentes `.razor`.
- `MainLayout.razor.css` é residual do template Blazor original e tem estilos não utilizados.

### 6.9 Sem testes automatizados

**Decisão:** Projeto não tem nenhum teste unitário ou de integração atualmente. Pasta de testes não existe.

**Impactos:**
- Qualquer refatoração deve ser verificada manualmente (compilar + testar endpoints no Swagger).
- Frameworks recomendados quando forem criados: **xUnit** para unitários, **Moq** para mocks, `WebApplicationFactory<T>` para integração.

### 6.10 Solution incompleta

**Decisão:** O arquivo `FitUP.slnx` referencia apenas o projeto front-end (`FitUP/FitUP.csproj`). O back-end (`src/FitUP.WebApi/FitUP.WebApi.csproj`) não está incluído.

**Impactos:**
- `dotnet build` na raiz não compila o back-end.
- Para compilar tudo: `dotnet build src/FitUP.WebApi && dotnet build FitUP`.
- Para adicionar o back-end: incluir `<Project Path="src/FitUP.WebApi/FitUP.WebApi.csproj" />` no `.slnx`.

---

*Documento mantido junto ao código. Atualizar quando decisões arquiteturais forem alteradas.*
