# FitUP — Documento de Visão do Produto

## 1. Propósito

O FitUP é uma plataforma web fitness que permite a usuários montar treinos personalizados, acompanhar planos alimentares e monitorar a evolução corporal. Projeto acadêmico com front-end Blazor WebAssembly (.NET 10) e back-end ASP.NET Core Web API (.NET 10) com SQL Server Express 2025.

---

## 2. Stack Tecnológica

### 2.1 Front-End

| Camada | Tecnologia | Versão | Função |
|---|---|---|---|
| Runtime | .NET | 10.0 | Base do Blazor WebAssembly |
| Framework UI | Blazor WebAssembly | 10.* | SPA rodando no navegador |
| Componentes | MudBlazor | 9.* | Material Design components |
| Estilização | CSS3 | — | Temas claro/escuro, responsividade |

### 2.2 Back-End

| Camada | Tecnologia | Versão | Função |
|---|---|---|---|
| Runtime | .NET | 10.0 | Base da Web API |
| Framework | ASP.NET Core | 10.* | API REST |
| Autenticação | JwtBearer | 10.* | Tokens JWT stateless |
| Hash de senhas | BCrypt.Net-Next | 4.* | Hash + salt |
| Acesso a dados | Microsoft.Data.SqlClient | 6.* | ADO.NET puro (sem ORM) |
| Documentação | Swashbuckle | 7.* | Swagger/OpenAPI |

### 2.3 Banco de Dados

| Componente | Versão | Função |
|---|---|---|
| SQL Server Express | 2025 | Banco relacional |
| Script SQL | — | Criação de 8 tabelas + índices |

---

## 3. Arquitetura do Sistema

```
┌──────────────────────────────────────────────────────────────────┐
│                    FRONT-END (Blazor WASM)                       │
│                                                                  │
│  FitUP/                                                         │
│  ├── Pages/          → 11 páginas .razor                        │
│  ├── Layout/         → MainLayout + NavMenu                     │
│  └── wwwroot/        → index.html, CSS, imagens                 │
│                                                                  │
│  Portas: HTTP 5059 / HTTPS 7211                                 │
└──────────────────────────┬───────────────────────────────────────┘
                           │
                    HTTP/JSON (REST)
                           │
                           ▼
┌──────────────────────────────────────────────────────────────────┐
│                    BACK-END (ASP.NET Core Web API)               │
│                                                                  │
│  src/FitUP.WebApi/                                              │
│  ├── Controllers/    → 5 controllers, 22 endpoints              │
│  ├── DTOs/           → 13 classes de entrada/saída              │
│  ├── Service/        → 6 interfaces + 6 implementações          │
│  ├── Models/         → 8 entidades POCO                         │
│  ├── Infrastructure/ → script.sql                               │
│  ├── Program.cs      → DI, JWT, CORS, Swagger                   │
│  └── appsettings.json → Connection string + JWT config          │
│                                                                  │
│  Portas: HTTP 5000 / HTTPS 5001                                 │
└──────────────────────────┬───────────────────────────────────────┘
                           │
                    ADO.NET (SqlClient)
                           │
                           ▼
┌──────────────────────────────────────────────────────────────────┐
│              SQL SERVER EXPRESS 2025                             │
│                                                                  │
│  Database: FitUP                                                │
│  Tabelas: Usuario, PlanoTreino, DiaTreino, Exercicio,           │
│           PlanoAlimentar, Refeicao, Alimento,                    │
│           RegistroBioimpedancia                                  │
└──────────────────────────────────────────────────────────────────┘
```

---

## 4. Modelagem de Dados — Relacionamentos

```
Usuario (1) ──< (N) PlanoTreino
Usuario (1) ──< (N) PlanoAlimentar
Usuario (1) ──< (N) RegistroBioimpedancia
PlanoTreino (1) ──< (N) DiaTreino
DiaTreino (1) ──< (N) Exercicio
PlanoAlimentar (1) ──< (N) Refeicao
Refeicao (1) ──< (N) Alimento
```

### 4.1 Entidade: Usuario

| Propriedade | Tipo | Restrição | Descrição |
|---|---|---|---|
| Id | UNIQUEIDENTIFIER | PK, DEFAULT NEWSEQUENTIALID() | Identificador único |
| Nome | NVARCHAR(100) | NOT NULL | Primeiro nome |
| Sobrenome | NVARCHAR(100) | NOT NULL | Sobrenome |
| Email | NVARCHAR(200) | NOT NULL, UNIQUE | Email de login |
| SenhaHash | NVARCHAR(MAX) | NOT NULL | Hash BCrypt da senha |
| Telefone | NVARCHAR(20) | NULL | Telefone opcional |
| CPF | NVARCHAR(14) | NULL | CPF opcional |
| DataNascimento | DATE | NULL | Data de nascimento |
| CriadoEm | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Data de criação |
| UltimoLoginEm | DATETIME2 | NULL | Último login |
| Ativo | BIT | NOT NULL, DEFAULT 1 | Conta ativa/inativa |
| RefreshToken | NVARCHAR(500) | NULL | Token de renovação |
| RefreshTokenExpiraEm | DATETIME2 | NULL | Expiração do refresh |

### 4.2 Entidade: PlanoTreino

| Propriedade | Tipo | Restrição | Descrição |
|---|---|---|---|
| Id | UNIQUEIDENTIFIER | PK | Identificador |
| UsuarioId | UNIQUEIDENTIFIER | FK → Usuario(Id) CASCADE | Dono do plano |
| Nome | NVARCHAR(150) | NOT NULL | Nome do plano |
| Descricao | NVARCHAR(500) | NULL | Descrição |
| Divisao | INT | NOT NULL, DEFAULT 0 | Enum: UpperLower, Fullbody, PPL, ABCD |
| Nivel | INT | NOT NULL, DEFAULT 0 | Enum: Iniciante, Intermediário, Avançado |
| FrequenciaSemanal | INT | NOT NULL, DEFAULT 0 | Dias por semana |
| CriadoEm | DATETIME2 | NOT NULL | Data de criação |
| AtualizadoEm | DATETIME2 | NULL | Última atualização |

### 4.3 Entidade: DiaTreino

| Propriedade | Tipo | Restrição | Descrição |
|---|---|---|---|
| Id | UNIQUEIDENTIFIER | PK | Identificador |
| PlanoTreinoId | UNIQUEIDENTIFIER | FK → PlanoTreino(Id) CASCADE | Plano pai |
| Nome | NVARCHAR(100) | NOT NULL | Ex: "Peito", "Costas" |
| Ordem | INT | NOT NULL, DEFAULT 0 | Ordem no plano |

### 4.4 Entidade: Exercicio

| Propriedade | Tipo | Restrição | Descrição |
|---|---|---|---|
| Id | UNIQUEIDENTIFIER | PK | Identificador |
| DiaTreinoId | UNIQUEIDENTIFIER | FK → DiaTreino(Id) CASCADE | Dia pai |
| Nome | NVARCHAR(150) | NOT NULL | Nome do exercício |
| Descricao | NVARCHAR(500) | NULL | Descrição/instruções |
| GrupoMuscular | INT | NOT NULL, DEFAULT 0 | Enum: Peito, Costas, Perna, etc. |
| Series | INT | NOT NULL, DEFAULT 0 | Número de séries |
| Repeticoes | INT | NOT NULL, DEFAULT 0 | Número de repetições |
| Carga | FLOAT | NULL | Carga em kg |
| Observacoes | NVARCHAR(500) | NULL | Observações adicionais |
| Ordem | INT | NOT NULL, DEFAULT 0 | Ordem no dia |

### 4.5 Entidade: PlanoAlimentar

| Propriedade | Tipo | Restrição | Descrição |
|---|---|---|---|
| Id | UNIQUEIDENTIFIER | PK | Identificador |
| UsuarioId | UNIQUEIDENTIFIER | FK → Usuario(Id) CASCADE | Dono do plano |
| Nome | NVARCHAR(150) | NOT NULL | Nome do plano |
| Objetivo | INT | NOT NULL, DEFAULT 0 | Enum: Bulking, Cutting, Manutenção |
| Descricao | NVARCHAR(500) | NULL | Descrição |
| CriadoEm | DATETIME2 | NOT NULL | Data de criação |
| AtualizadoEm | DATETIME2 | NULL | Última atualização |

### 4.6 Entidade: Refeicao

| Propriedade | Tipo | Restrição | Descrição |
|---|---|---|---|
| Id | UNIQUEIDENTIFIER | PK | Identificador |
| PlanoAlimentarId | UNIQUEIDENTIFIER | FK → PlanoAlimentar(Id) CASCADE | Plano pai |
| Nome | NVARCHAR(100) | NOT NULL | Ex: "Café da manhã" |
| HorarioSugerido | TIME | NULL | Horário recomendado |
| Ordem | INT | NOT NULL, DEFAULT 0 | Ordem no plano |
| TotalProteina | FLOAT | NOT NULL, DEFAULT 0 | Soma das proteínas |
| TotalCarboidrato | FLOAT | NOT NULL, DEFAULT 0 | Soma dos carboidratos |
| TotalGordura | FLOAT | NOT NULL, DEFAULT 0 | Soma das gorduras |
| TotalFibra | FLOAT | NOT NULL, DEFAULT 0 | Soma das fibras |
| TotalCalorias | FLOAT | NOT NULL, DEFAULT 0 | Soma das calorias |

### 4.7 Entidade: Alimento

| Propriedade | Tipo | Restrição | Descrição |
|---|---|---|---|
| Id | UNIQUEIDENTIFIER | PK | Identificador |
| RefeicaoId | UNIQUEIDENTIFIER | FK → Refeicao(Id) CASCADE | Refeição pai |
| Nome | NVARCHAR(150) | NOT NULL | Nome do alimento |
| Quantidade | FLOAT | NOT NULL, DEFAULT 0 | Quantidade |
| UnidadeMedida | NVARCHAR(20) | NOT NULL, DEFAULT 'g' | g, ml, unidade |
| Proteina | FLOAT | NOT NULL, DEFAULT 0 | Gramas de proteína |
| Carboidrato | FLOAT | NOT NULL, DEFAULT 0 | Gramas de carboidrato |
| Gordura | FLOAT | NOT NULL, DEFAULT 0 | Gramas de gordura |
| Fibra | FLOAT | NOT NULL, DEFAULT 0 | Gramas de fibra |
| Calorias | FLOAT | NOT NULL, DEFAULT 0 | Calorias |
| Observacoes | NVARCHAR(300) | NULL | Observações |

### 4.8 Entidade: RegistroBioimpedancia

| Propriedade | Tipo | Restrição | Descrição |
|---|---|---|---|
| Id | UNIQUEIDENTIFIER | PK | Identificador |
| UsuarioId | UNIQUEIDENTIFIER | FK → Usuario(Id) CASCADE | Dono do registro |
| DataRegistro | DATETIME2 | NOT NULL, DEFAULT GETUTCDATE() | Data da medição |
| Peso | FLOAT | NOT NULL, DEFAULT 0 | Peso em kg |
| Altura | FLOAT | NOT NULL, DEFAULT 0 | Altura em cm |
| MassaMagra | FLOAT | NULL | Massa magra em kg |
| MassaGorda | FLOAT | NULL | Massa gorda em kg |
| PercentualGordura | FLOAT | NULL | % de gordura corporal |
| MassaMuscular | FLOAT | NULL | Massa muscular em kg |
| AguaCorporal | FLOAT | NULL | Água corporal em L |
| TaxaMetabolicaBasal | FLOAT | NULL | TMB em kcal |
| IdadeMetabolica | FLOAT | NULL | Idade metabólica |
| CircunferenciaCintura | FLOAT | NULL | Cintura em cm |
| CircunferenciaQuadril | FLOAT | NULL | Quadril em cm |
| RelacaoCinturaQuadril | FLOAT | NULL | Relação cintura/quadril |
| Observacoes | NVARCHAR(500) | NULL | Observações |

---

## 5. API — Endpoints

### 5.1 Autenticação (Pública)

| Método | Rota | Request Body | Response | Descrição |
|---|---|---|---|---|
| POST | /api/auth/login | LoginRequest | AuthResponse | Login com email + senha |
| POST | /api/auth/registrar | RegistroRequest | AuthResponse | Cadastro de novo usuário |
| POST | /api/auth/refresh-token | RefreshTokenRequest | AuthResponse | Renovar token expirado |

### 5.2 Usuário (Protegida)

| Método | Rota | Request Body | Response | Descrição |
|---|---|---|---|---|
| GET | /api/usuario/me | — | UsuarioDto | Dados do perfil |
| PUT | /api/usuario/me | UsuarioUpdateRequest | UsuarioDto | Atualizar perfil |
| PUT | /api/usuario/me/alterar-senha | AlterarSenhaRequest | 204 No Content | Trocar senha |
| DELETE | /api/usuario/me | — | 204 No Content | Desativar conta |

### 5.3 Planos de Treino (Protegida)

| Método | Rota | Request Body | Response | Descrição |
|---|---|---|---|---|
| GET | /api/planos-treino | — | List<PlanoTreinoDto> | Listar planos |
| GET | /api/planos-treino/{id} | — | PlanoTreinoDto | Obter plano completo |
| POST | /api/planos-treino | PlanoTreinoRequest | PlanoTreinoDto | Criar plano |
| PUT | /api/planos-treino/{id} | PlanoTreinoRequest | PlanoTreinoDto | Atualizar plano |
| DELETE | /api/planos-treino/{id} | — | 204 No Content | Remover plano |

### 5.4 Planos Alimentares (Protegida)

| Método | Rota | Request Body | Response | Descrição |
|---|---|---|---|---|
| GET | /api/planos-alimentares | — | List<PlanoAlimentarDto> | Listar planos |
| GET | /api/planos-alimentares/{id} | — | PlanoAlimentarDto | Obter plano completo |
| POST | /api/planos-alimentares | PlanoAlimentarRequest | PlanoAlimentarDto | Criar plano |
| PUT | /api/planos-alimentares/{id} | PlanoAlimentarRequest | PlanoAlimentarDto | Atualizar plano |
| DELETE | /api/planos-alimentares/{id} | — | 204 No Content | Remover plano |

### 5.5 Bioimpedância (Protegida)

| Método | Rota | Request Body | Response | Descrição |
|---|---|---|---|---|
| GET | /api/bioimpedancia | — | List<RegistroBioimpedanciaDto> | Listar registros |
| GET | /api/bioimpedancia/{id} | — | RegistroBioimpedanciaDto | Obter registro |
| POST | /api/bioimpedancia | RegistroBioimpedanciaRequest | RegistroBioimpedanciaDto | Criar registro |
| PUT | /api/bioimpedancia/{id} | RegistroBioimpedanciaRequest | RegistroBioimpedanciaDto | Atualizar registro |
| DELETE | /api/bioimpedancia/{id} | — | 204 No Content | Remover registro |

**Total: 22 endpoints** (3 públicos + 19 protegidos)

---

## 6. Fluxo de Autenticação

```
[Usuário] → POST /api/auth/login { email, senha }
              ↓
         AuthService.LoginAsync()
              ↓
         Busca usuário por email (SELECT com SqlParameter)
              ↓
         BCrypt.Verify(senha, usuario.SenhaHash)
              ↓
         [Falha] → return null → 401 Unauthorized
              ↓
         [Sucesso] → TokenService.GerarTokenJwt(usuario)
              ↓
         Gera JWT com claims: sub (Id), email, nome
         Gera RefreshToken com RandomNumberGenerator (64 bytes)
              ↓
         Atualiza RefreshToken e UltimoLoginEm no banco
              ↓
         Retorna AuthResponse { token, refreshToken, expiraEm }
              ↓
[Front-end] → Armazena token no localStorage
              ↓
         Envia token no header: Authorization: Bearer {token}
              ↓
[Back-end] → JwtBearer middleware valida:
              - Issuer, Audience, Lifetime, SigningKey
              - ClockSkew = TimeSpan.Zero
              ↓
         [Token expirado] → 401 → Front-end usa refresh-token
```

---

## 7. Front-End — Páginas

### 7.1 Páginas Implementadas

| Nome | Rota | Funcionalidade | Componentes MudBlazor |
|---|---|---|---|
| Home | `/` | Landing page com cards de treinos (Upper/Lower, Fullbody, PPL, ABCD, Alimentação) | MudCard, MudCardMedia, MudButton, MudGrid, MudIcon |
| Login | `/login` | Formulário de login com validação e toggle de senha | MudPaper, MudTextField, MudButton, EditForm |
| Cadastro | `/cadastrar` | Formulário de cadastro (Nome, Sobrenome, CPF com máscara, Telefone, Data, Email, Senha) | MudDatePicker, MudTextField com PatternMask, EditForm |
| DicasTreino | `/DicasTreino` | Guia de métodos de treino (Drop Set, Pirâmide, Bi-Set, Rest-Pause, etc.) com diálogos | MudDialog, MudCard, MudButton |
| GanhoMaximo | `/GanhoMaximo` | Guia alimentar com steppers (Bulking, Cutting, Manutenção) e planos de refeição com imagens | MudStepper, MudStep, MudCard, MudImage |
| CalculadoraBio | `/CalculadoraBio` | Calculadora de bioimpedância com inputs e resultados locais | MudTextField, MudButton, MudCard |
| MonteTreino | `/MonteTreino` | Placeholder "Em construção" | MudText, MudIcon |
| Indicacoes | `/Indicacoes` | Recomendações de academias com links externos | MudCard, MudLink, MudIconButton |
| NotFound | — | Página 404 com botão "Voltar" | MudCard, MudButton |

### 7.2 Tema Visual

| Propriedade | Modo Escuro (padrão) | Modo Claro |
|---|---|---|
| Primary | `#5e3103` | `#ededed` |
| Surface | `#1f1d1b` | `#e8e2da` |
| Background | `#000000` | — |
| TextPrimary | `#ffffff` | `#000000` |
| DrawerIcon | `#ff7f50` (coral) | `#ff7f50` (coral) |
| Secondary | `#2e8b57` | `#2e8b57` |
| Error | `#8B0000` | `#8B0000` |
| Warning | `#f4a460` | `#f4a460` |
| Success | `#008B8B` | `#008B8B` |

---

## 8. Estrutura de Diretórios

```
FitUP/                              # Raiz do repositório
│
├── docs/                           # Documentação
│   └── visao.md                    # ← Este arquivo
│
├── FitUP/                          # Front-end Blazor WebAssembly
│   ├── Pages/                      # 11 arquivos .razor
│   │   ├── Home.razor              # Landing page
│   │   ├── Login.razor             # Login
│   │   ├── Cadastro.razor          # Cadastro
│   │   ├── DicasTreino.razor       # Guia de treinos
│   │   ├── GanhoMaximo.razor       # Guia alimentar
│   │   ├── CalculadoraBio.razor    # Bioimpedância
│   │   ├── MonteTreino.razor       # Em construção
│   │   ├── Indicacoes.razor        # Recomendações
│   │   ├── MensagemCustomizada.razor # Diálogo reutilizável
│   │   └── NotFound.razor          # 404
│   ├── Layout/
│   │   ├── MainLayout.razor        # Layout principal (tema, appbar, footer)
│   │   ├── MainLayout.razor.css    # Estilos do layout
│   │   └── NavMenu.razor           # Menu de navegação
│   ├── Properties/
│   │   └── launchSettings.json     # Portas 5059/7211
│   ├── wwwroot/
│   │   ├── index.html              # SPA entry point
│   │   ├── css/app.css             # Estilos globais
│   │   ├── img/                    # Imagens do site
│   │   ├── img-gm/                 # Imagens do Ganho Máximo
│   │   └── sample-data/weather.json
│   ├── Program.cs                  # Entry point front-end
│   ├── App.razor                   # Router + NotFound
│   ├── _Imports.razor              # Global usings
│   └── FitUP.csproj                # net10.0, MudBlazor 9.*
│
├── src/                            # Código fonte back-end
│   └── FitUP.WebApi/               # Web API
│       ├── Controllers/
│       │   ├── AuthController.cs           # 3 endpoints (público)
│       │   ├── UsuarioController.cs        # 4 endpoints
│       │   ├── PlanoTreinoController.cs    # 5 endpoints
│       │   ├── PlanoAlimentarController.cs # 5 endpoints
│       │   └── RegistroBioimpedanciaController.cs # 5 endpoints
│       ├── DTOs/
│       │   ├── LoginRequest.cs
│       │   ├── RegistroRequest.cs
│       │   ├── AuthResponse.cs
│       │   ├── RefreshTokenRequest.cs
│       │   ├── UsuarioDto.cs
│       │   ├── UsuarioUpdateRequest.cs
│       │   ├── AlterarSenhaRequest.cs
│       │   ├── PlanoTreinoDto.cs
│       │   ├── PlanoTreinoRequest.cs
│       │   ├── PlanoAlimentarDto.cs
│       │   ├── PlanoAlimentarRequest.cs
│       │   ├── RegistroBioimpedanciaDto.cs
│       │   └── RegistroBioimpedanciaRequest.cs
│       ├── Infrastructure/
│       │   └── script.sql                  # 8 tabelas + índices
│       ├── Models/
│       │   ├── Usuario.cs
│       │   ├── PlanoTreino.cs
│       │   ├── DiaTreino.cs
│       │   ├── Exercicio.cs
│       │   ├── PlanoAlimentar.cs
│       │   ├── Refeicao.cs
│       │   ├── Alimento.cs
│       │   └── RegistroBioimpedancia.cs
│       ├── Properties/
│       │   └── launchSettings.json         # Portas 5000/5001
│       ├── Service/
│       │   ├── ITokenService.cs / TokenService.cs
│       │   ├── IAuthService.cs / AuthService.cs
│       │   ├── IUsuarioService.cs / UsuarioService.cs
│       │   ├── IPlanoTreinoService.cs / PlanoTreinoService.cs
│       │   ├── IPlanoAlimentarService.cs / PlanoAlimentarService.cs
│       │   └── IRegistroBioimpedanciaService.cs / RegistroBioimpedanciaService.cs
│       ├── Program.cs                      # DI, JWT, CORS, Swagger
│       ├── appsettings.json                # Connection string + JWT
│       └── FitUP.WebApi.csproj             # net10.0, SqlClient, BCrypt, JwtBearer, Swashbuckle
│
├── FitUP.slnx                              # Solution file
├── README.md
└── .gitignore
```

---

## 9. Injeção de Dependência (Program.cs)

```csharp
// Serviços registrados via AddScoped (uma instância por requisição HTTP)
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPlanoTreinoService, PlanoTreinoService>();
builder.Services.AddScoped<IPlanoAlimentarService, PlanoAlimentarService>();
builder.Services.AddScoped<IRegistroBioimpedanciaService, RegistroBioimpedanciaService>();
```

### 9.1 Cadeia de Dependências

```
Controller → Service (interface) → Service (implementação) → SqlConnection
                                                                  ↓
                                                            script.sql (tabelas)
```

Cada Service recebe `IConfiguration` via construtor para ler a connection string e cria `SqlConnection` próprias (stateless).

---

## 10. Segurança

| Aspecto | Implementação |
|---|---|
| Hash de senhas | BCrypt.Net-Next (custo padrão 10-12) |
| Autenticação | JWT Bearer Token (120 min de expiração) |
| Renovação | Refresh Token (64 bytes randômicos, rotativo) |
| SQL Injection | Parâmetros via SqlParameter em todas as queries |
| CORS | Apenas origens: http://localhost:5059, https://localhost:7211 |
| Rotas públicas | Apenas 3 endpoints de auth |
| Rotas protegidas | 19 endpoints com [Authorize] |
| Validação de claims | Controller obtém UsuarioId do ClaimTypes.NameIdentifier |

---

## 11. Roadmap

### ✅ Concluído (Fase 1)

- [x] Estrutura de diretórios do back-end
- [x] 8 Models (POCOs sem navegação)
- [x] script.sql com 8 tabelas, FKs com CASCADE, índices
- [x] Program.cs com JWT, Swagger, CORS, DI
- [x] appsettings.json com connection string e JWT config
- [x] 13 DTOs de entrada/saída
- [x] 6 Services (interface + implementação) com ADO.NET
- [x] 5 Controllers com 22 endpoints
- [x] Compilação com 0 erros e 0 warnings
- [x] 3 commits no branch devpedro (Ianclrs/FitUP)

### 🔄 Próximos Passos (Fase 2)

- [ ] Criar serviços HTTP no front-end (AuthService, UsuarioService, etc.)
- [ ] Implementar login real consumindo `/api/auth/login`
- [ ] Implementar cadastro real consumindo `/api/auth/registrar`
- [ ] Armazenar JWT no localStorage e anexar às requisições
- [ ] Implementar refresh token automático
- [ ] Criar páginas de gerenciamento de planos de treino
- [ ] Criar páginas de gerenciamento de planos alimentares
- [ ] Criar página de histórico de bioimpedância com gráficos

### 📋 Planejado (Fase 3)

- [ ] Dashboard com gráficos de evolução (MudChart)
- [ ] Upload de foto de perfil
- [ ] Validações avançadas e feedback visual (Snackbar)
- [ ] Testes unitários nos Services
- [ ] Deploy (Azure ou outra cloud)

---

## 12. Configuração para Desenvolvimento Local

### Pré-requisitos

```bash
# Verificar versões
dotnet --version                    # Deve retornar 10.x
sqlcmd -?                           # SQL Server Express 2025 instalado
```

### Passo a Passo

```bash
# 1. Clonar o repositório
git clone https://github.com/Ianclrs/FitUP.git
cd FitUP

# 2. Criar banco de dados
# Executar src/FitUP.WebApi/Infrastructure/script.sql no SQL Server Management Studio ou sqlcmd

# 3. Configurar connection string
# Editar src/FitUP.WebApi/appsettings.json:
# "ConnectionStrings:DefaultConnection": "Server=.\\SQLEXPRESS;Database=FitUP;Trusted_Connection=True;TrustServerCertificate=True;"

# 4. Executar back-end
cd src/FitUP.WebApi
dotnet run
# API em http://localhost:5000
# Swagger em http://localhost:5000/swagger

# 5. Executar front-end (novo terminal)
cd FitUP
dotnet run
# App em http://localhost:5059
```

---

## 13. Informações do Repositório

| Item | Valor |
|---|---|
| Repositório | github.com/Ianclrs/FitUP |
| Branch ativa | devpedro |
| Pull Request | #6 (aberto) |
| Commits | 3 (estrutura, DTOs+Services, Controllers) |
| Front-end | Blazor WebAssembly (pasta FitUP/) |
| Back-end | ASP.NET Core Web API (pasta src/FitUP.WebApi/) |

---

## 14. Contribuidores

Projeto acadêmico — Instituto Federal de São Paulo (IFSP)

- **Ian** — [github.com/Ianclrs](https://github.com/Ianclrs)
- **Pedro** — [github.com/pdfreitass](https://github.com/pdfreitass)

---

*Documento gerado em Maio de 2026. Mantenha atualizado conforme o projeto evolui.*
