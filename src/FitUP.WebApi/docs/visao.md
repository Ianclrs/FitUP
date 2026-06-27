# FitUP

## 1. O que é

Plataforma web de fitness. Usuários montam planos de treino, planos alimentares e acompanham evolução corporal (bioimpedância).

**Projeto acadêmico** — Instituto Federal de São Paulo (IFSP).  
**Público-alvo:** qualquer pessoa que treina e quer organizar treino/dieta/medições num lugar só.  
**Problema resolvido:** substitui planilhas soltas e apps genéricos por um sistema integrado e pessoal.

## 2. Estado atual

**MVP em desenvolvimento — Fase 1 concluída, Fase 2 em andamento.**

| Camada | Status |
|---|---|
| Banco de dados (SQL Server) | ✅ 8 tabelas, FKs, índices |
| Back-end API (.NET 10) | ✅ 22 endpoints, autenticação JWT, compila sem erros |
| Front-end Blazor WASM | 🔄 Páginas estáticas prontas. Login/cadastro integrados à API. **Falta toda a parte autenticada (CRUD de planos, bioimpedância, dashboard).** |

## 3. Funcionalidades

- **Autenticação:** cadastro, login, refresh token (JWT + BCrypt)
- **Planos de treino:** CRUD, com dias de treino e exercícios aninhados (séries, repetições, carga, grupo muscular)
- **Planos alimentares:** CRUD, com refeições e alimentos aninhados (macronutrientes por alimento, totais por refeição)
- **Bioimpedância:** CRUD de registros com 14 campos (peso, altura, massa magra/gordura/muscular, % gordura, TMB, circunferências, etc.)
- **Conteúdo informativo:** páginas estáticas com dicas de treino, guias de alimentação, calculadora local de bioimpedância

## 4. Arquitetura

```
FitUP/                          ← Front-end Blazor WASM (net10.0, MudBlazor 9.*)
  └── HTTP REST (localhost:5000) → JWT no header Authorization

src/FitUP.WebApi/               ← Back-end ASP.NET Core (net10.0)
  └── ADO.NET SqlClient → SQL Server Express 2025
```

**Padrão:** Controller → IService → Service → SqlConnection (criada por método, sem pooling explícito).  
**Autenticação:** JWT Bearer (120 min) + refresh token rotativo (7 dias). Senhas: BCrypt.  
**Acesso a dados:** ADO.NET puro, queries 100% parametrizadas com `SqlParameter`.  
**CORS:** liberado para `localhost:5059` e `localhost:7211`.

### Portas

| Componente | HTTP | HTTPS |
|---|---|---|
| Front-end | 5059 | 7211 |
| Back-end | 5000 | 5001 |

### Diagrama de dados (8 tabelas)

```
Usuario ──< PlanoTreino ──< DiaTreino ──< Exercicio
Usuario ──< PlanoAlimentar ──< Refeicao ──< Alimento
Usuario ──< RegistroBioimpedancia
```

Todas PKs: `UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID()`. Todas FKs: `ON DELETE CASCADE`.

## 5. Estrutura de diretórios

```
FitUP/
├── FitUP/                         ← Front-end Blazor WASM
│   ├── Pages/         11 .razor   ← Páginas (Home, Login, Cadastro, Dicas, etc.)
│   ├── Layout/        2 .razor    ← MainLayout + NavMenu
│   ├── Services/      1 .cs       ← AuthService (HTTP client para API)
│   ├── wwwroot/                   ← CSS, imagens, index.html
│   ├── Program.cs                 ← Entry point, DI (MudBlazor, HttpClient)
│   └── FitUP.csproj               ← MudBlazor 9.*, Blazor WASM 10.*
│
├── src/FitUP.WebApi/              ← Back-end API
│   ├── Controllers/   5 .cs       ← Auth, Usuario, PlanoTreino, PlanoAlimentar, Bioimpedancia
│   ├── Service/       12 .cs      ← 6 interfaces + 6 implementações (ADO.NET)
│   ├── DTOs/          13 .cs      ← Request/Response
│   ├── Models/        8 .cs       ← POCOs sem navegação
│   ├── Infrastructure/ script.sql ← Schema do banco (8 tabelas + índices)
│   ├── Program.cs                 ← JWT, Swagger, CORS, DI (tudo Scoped)
│   ├── appsettings.json           ← Connection string + JWT config
│   ├── docs/visao.md              ← Este arquivo
│   └── FitUP.WebApi.csproj        ← SqlClient 6.*, BCrypt 4.*, JwtBearer 10.*, Swashbuckle 7.*
│
├── FitUP.slnx                     ← Solution (referencia apenas o front-end)
└── README.md
```

## 6. Como rodar

```bash
# Pré-requisitos: .NET SDK 10.0, SQL Server Express 2025

# 1. Criar banco → executar src/FitUP.WebApi/Infrastructure/script.sql

# 2. Back-end (terminal 1)
cd src/FitUP.WebApi && dotnet run
# → http://localhost:5000 | Swagger: http://localhost:5000/swagger

# 3. Front-end (terminal 2)
cd FitUP && dotnet run
# → http://localhost:5059
```

## 7. Bugs conhecidos (back-end)

1. **TokenService.cs:27** — Lê chave `ExpireMinutes` mas `appsettings.json` define `ExpiresInMinutes`. Config nunca é aplicada.
2. **AuthService.cs:63 e 191** — `reader.Close()` manual antes do UPDATE. Se UPDATE falhar, refresh token não persiste mas JWT já foi gerado.
3. **AuthService.RegistrarAsync** — Sem validação server-side (tamanho de senha, formato de email, CPF, data futura). Confia só no front-end.
4. **PlanoTreinoService.ListarPorUsuarioAsync** — N+1 queries. Para N planos, abre N+1 conexões.

## 8. API — Resumo

| Controller | Rota | Endpoints | Auth |
|---|---|---|---|
| Auth | `/api/auth` | login, registrar, refresh-token | Público |
| Usuario | `/api/usuario` | me (GET/PUT/DELETE), alterar-senha | JWT |
| PlanoTreino | `/api/planos-treino` | CRUD (5 endpoints) | JWT |
| PlanoAlimentar | `/api/planos-alimentares` | CRUD (5 endpoints) | JWT |
| Bioimpedancia | `/api/bioimpedancia` | CRUD (5 endpoints) | JWT |

**Total:** 22 endpoints (3 públicos, 19 protegidos).  
**Formato de erro:** `{ "mensagem": "string" }`.  
**Para contratos completos (request/response JSON):** ler os DTOs em `src/FitUP.WebApi/DTOs/`.

## 9. O que precisa ser feito agora

### Prioridade: corrigir bugs do back-end
- Corrigir chave `ExpiresInMinutes` no TokenService
- Remover `reader.Close()` manual no AuthService
- Adicionar validação server-side no registro
- Corrigir N+1 no ListarPorUsuarioAsync

### Prioridade: completar integração front-end ↔ API
- Resolver conflito de merge em `FitUP/Pages/Cadastro.razor` (marcadores `<<<<<<< HEAD`)
- Armazenar JWT no localStorage e injetar em chamadas autenticadas
- Criar `AuthenticationStateProvider` + interceptor de refresh token
- Criar páginas CRUD: meus-treinos, meus-planos, minha-evolucao
- Adicionar `src/FitUP.WebApi` ao `FitUP.slnx`

### Depois
- Dashboard com MudChart
- Upload de foto de perfil
- Testes unitários e de integração
- Deploy

---

*Atualizado em Junho/2026. Branch: devpedro. Repositório: github.com/Ianclrs/FitUP.*
