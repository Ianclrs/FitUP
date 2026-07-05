<br/>
<div align="center">
  <h1>🏋️‍♂️ FitUP</h1>
  <p>
    <strong>Plataforma Fitness Inteligente — Treinos, Nutrição e Evolução em um só lugar.</strong>
  </p>
  <p>
    <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 10"/>
    <img src="https://img.shields.io/badge/Blazor-WebAssembly-512BD4?style=flat-square&logo=blazor&logoColor=white" alt="Blazor WASM"/>
    <img src="https://img.shields.io/badge/MudBlazor-9.x-FF6A00?style=flat-square&logo=mudblazor&logoColor=white" alt="MudBlazor"/>
    <img src="https://img.shields.io/badge/SQL_Server-2019+-CC2927?style=flat-square&logo=microsoft-sql-server&logoColor=white" alt="SQL Server"/>
    <img src="https://img.shields.io/badge/JWT-Auth-000000?style=flat-square&logo=json-web-tokens&logoColor=white" alt="JWT Auth"/>
    <img src="https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=flat-square&logo=swagger&logoColor=black" alt="Swagger"/>
    <img src="https://img.shields.io/badge/BCrypt-Next-4B8BBE?style=flat-square&logo=letsencrypt&logoColor=white" alt="BCrypt"/>
    <img src="https://img.shields.io/badge/License-MIT-yellow?style=flat-square" alt="License"/>
  </p>
</div>

---

## 📖 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Arquitetura](#-arquitetura)
- [Tecnologias](#-tecnologias)
- [Funcionalidades](#-funcionalidades)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Banco de Dados](#-banco-de-dados)
- [Como Executar](#-como-executar)
- [API Endpoints](#-api-endpoints)
- [Decisões de Arquitetura (ADR)](#-decisões-de-arquitetura-adr)
- [Práticas de Segurança](#-práticas-de-segurança)
- [Equipe](#-equipe)
- [Licença](#-licença)

---

## 💪 Sobre o Projeto

O **FitUP** é uma aplicação web full-stack projetada para atender entusiastas de fitness, atletas e profissionais da saúde que buscam centralizar o gerenciamento de treinos, planos alimentares e métricas corporais. A plataforma oferece:

- **Montagem interativa de treinos** com divisões clássicas (Fullbody, PPL, Upper/Lower)
- **Calculadora de macronutrientes** com distribuição automatizada baseada em objetivos (hipertrofia, definição, manutenção)
- **Análise de bioimpedância** com cálculo de IMC, TMB e composição corporal
- **Perfil do usuário** com alteração de nome, e-mail e senha
- **Autenticação robusta** via JWT + Refresh Token com senhas hasheadas em BCrypt

O projeto foi concebido como trabalho acadêmico da disciplina de **Engenharia de Software**, aplicando metodologias ágeis (Scrum), arquitetura limpa e boas práticas de desenvolvimento.

---

## 🏗️ Arquitetura

```
┌──────────────────────────────────────────────────────────────┐
│                     CLIENTE (Navegador)                       │
│  ┌────────────────────────────────────────────────────────┐  │
│  │              FitUP (Blazor WebAssembly)                 │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐  │  │
│  │  │  Pages   │ │  Layout  │ │ Services │ │ MudBlazor│  │  │
│  │  │ (Razor)  │ │ (Razor)  │ │  (C#)    │ │   (UI)   │  │  │
│  │  └──────────┘ └──────────┘ └──────────┘ └──────────┘  │  │
│  └────────────────────┬───────────────────────────────────┘  │
│                       │ HTTP (REST/JSON)                     │
└───────────────────────┼──────────────────────────────────────┘
                        │
┌───────────────────────┼──────────────────────────────────────┐
│                 BACKEND (Servidor)                            │
│  ┌────────────────────┴───────────────────────────────────┐  │
│  │           FitUP.WebApi (ASP.NET Core)                   │  │
│  │  ┌────────────┐ ┌──────────┐ ┌──────────────────────┐  │  │
│  │  │Controllers │ │ Services │ │     Middleware        │  │  │
│  │  │  (REST)    │ │ (Lógica) │ │ JWT · CORS · Swagger │  │  │
│  │  └────────────┘ └──────────┘ └──────────────────────┘  │  │
│  └────────────────────┬───────────────────────────────────┘  │
│                       │ ADO.NET (SqlClient)                  │
│  ┌────────────────────┴───────────────────────────────────┐  │
│  │                SQL Server                               │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────────┐           │  │
│  │  │ Usuario  │ │PlanoTrn.│ │ PlanoAliment.│  ...      │  │
│  │  └──────────┘ └──────────┘ └──────────────┘           │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
```

### Camadas

| Camada | Responsabilidade | Tecnologia |
|--------|-----------------|------------|
| **Apresentação (Frontend)** | Interface do usuário, validação client-side, estado de autenticação local | Blazor WebAssembly + MudBlazor |
| **API (Backend)** | Endpoints REST, autenticação/autorização, validação server-side | ASP.NET Core Web API |
| **Serviço** | Regras de negócio, hashing de senhas, geração de tokens | C# (injeção de dependência) |
| **Dados** | Persistência, consultas parametrizadas, transações | ADO.NET (Microsoft.Data.SqlClient) |

### Padrões e Princípios

- **Clean Architecture** — Separação clara entre controllers, serviços e acesso a dados
- **DTO Pattern** — Objetos de transferência isolam a camada de apresentação do modelo de domínio
- **Repository Pattern** — Serviços encapsulam queries SQL, mantendo controllers enxutos
- **Dependency Injection** — Todos os serviços são registrados no container DI do ASP.NET Core
- **Soft Delete** — Usuários são desativados (`Ativo = 0`) em vez de removidos fisicamente

---

## 🚀 Tecnologias

### Frontend (Blazor WebAssembly)

| Tecnologia | Versão | Finalidade |
|-----------|--------|------------|
| **.NET** | 10.0 | Runtime e SDK |
| **Blazor WebAssembly** | 10.0 | Framework SPA em C# (compilado para WebAssembly) |
| **MudBlazor** | 9.x | Biblioteca de componentes UI Material Design |
| **System.Net.Http.Json** | — | Serialização/deserialização JSON para chamadas à API |
| **IJSRuntime** | — | Interoperabilidade JavaScript para localStorage |
| **Microsoft.AspNetCore.Components.WebAssembly** | 10.0 | Hospedagem e tooling do Blazor WASM |

### Backend (ASP.NET Core Web API)

| Tecnologia | Versão | Finalidade |
|-----------|--------|------------|
| **.NET** | 10.0 | Runtime e SDK |
| **ASP.NET Core** | 10.0 | Framework web para construção da API REST |
| **Microsoft.Data.SqlClient** | 6.x | Driver ADO.NET para SQL Server |
| **BCrypt.Net-Next** | 4.x | Hashing seguro de senhas (sal + hash adaptativo) |
| **Microsoft.AspNetCore.Authentication.JwtBearer** | 10.0 | Autenticação e validação de tokens JWT |
| **Swashbuckle.AspNetCore** | 7.x | Geração de documentação OpenAPI / Swagger UI |

### Banco de Dados

| Tecnologia | Finalidade |
|-----------|------------|
| **Microsoft SQL Server** | SGBD relacional |
| **T-SQL** | Stored procedures, consultas parametrizadas |
| **Índices clusterizados (PK)** | Performance em buscas por GUID |

---

## ✨ Funcionalidades

### 🔐 Autenticação e Perfil

- **Registro de usuário** com nome, sobrenome, e-mail, senha, telefone, CPF e data de nascimento
- **Login** com e-mail e senha — retorna JWT + Refresh Token
- **Renovação de token** via endpoint `refresh-token` (expiração de 7 dias)
- **Perfil do usuário**:
  - 📛 **Alterar nome** — separação automática nome/sobrenome, persistido via `PUT api/usuario/me`
  - 📧 **Alterar e-mail** — validação com confirmação, atualização parcial via `COALESCE`
  - 🔒 **Alterar senha** — verificação da senha atual com BCrypt, persistência do novo hash
- **Logout** — limpeza de estado em memória, localStorage e header Authorization
- **Persistência de sessão** — sessão restaurada automaticamente ao recarregar a página (localStorage)
- **Soft delete de conta** — desativação lógica (`Ativo = 0`) sem perda de dados

### 🏋️ Treinos

- **Monte seu Treino** — ferramenta interativa com seleção de:
  - Divisão de treino (Fullbody, Push/Pull/Legs, Upper/Lower, etc.)
  - Nível de experiência (Iniciante, Intermediário, Avançado)
  - Objetivo (Hipertrofia, Força, Resistência, Emagrecimento)
  - Frequência semanal e duração da sessão
- **Treinos Salvos** — visualização dos planos de treino gerados, com exercícios, séries e repetições
- **Dicas de Treino** — conteúdo educativo com orientações para diferentes objetivos

### 🍎 Nutrição

- **Ganho Máximo** — calculadora de macronutrientes com:
  - Entrada de peso, altura, idade, sexo e nível de atividade
  - Cálculo automático de TMB (Taxa Metabólica Basal) por Harris-Benedict
  - Distribuição de proteínas, carboidratos e gorduras por objetivo
- **Calculadora de Bioimpedância** — registro e análise de:
  - Peso, percentual de gordura, massa magra, massa gorda
  - IMC e classificação
  - Histórico de medições

### 📄 Páginas Institucionais

- **Indicações** — recomendações e boas práticas
- **Termos de Uso** — termos legais da plataforma
- **Política de Privacidade** — política de tratamento de dados

### 🎨 Interface

- **Modo escuro/claro** — toggle persistente durante a sessão
- **Design responsivo** — adaptação para desktop, tablet e mobile
- **Snackbars** — feedback visual para ações do usuário (sucesso, erro)
- **Menu lateral** — drawer para navegação em telas menores
- **Footer** — links para redes sociais (LinkedIn, GitHub, Instagram) e páginas legais

---

## 📁 Estrutura do Projeto

```
FitUP/
├── FitUP.slnx                              # Solution file (.NET 10)
│
├── FitUP/                                  # 🖥️ Frontend — Blazor WebAssembly
│   ├── FitUP.csproj                        #   Dependências: MudBlazor, Blazor WASM
│   ├── Program.cs                          #   Bootstrap da aplicação, DI, HttpClient
│   ├── App.razor                           #   Componente raiz com roteamento
│   ├── _Imports.razor                      #   Usings globais dos componentes
│   ├── Pages/                              #   📄 Páginas Razor
│   │   ├── Home.razor                      #     Dashboard principal
│   │   ├── Login.razor                     #     Autenticação
│   │   ├── Cadastro.razor                  #     Registro de novo usuário
│   │   ├── Perfil.razor                    #     Alterar nome, e-mail e senha
│   │   ├── MonteTreino.razor               #     Montador interativo de treinos
│   │   ├── DicasTreino.razor               #     Guia educativo de treinos
│   │   ├── GanhoMaximo.razor               #     Calculadora de macronutrientes
│   │   ├── CalculadoraBio.razor            #     Análise de bioimpedância
│   │   ├── TreinosSalvos.razor             #     Planos de treino salvos
│   │   ├── MensagemCustomizada.razor       #     Diálogo modal reutilizável
│   │   ├── Indicacoes.razor                #     Recomendações
│   │   ├── Privacidade.razor               #     Política de privacidade
│   │   ├── Termo&Uso.razor                 #     Termos de uso
│   │   └── NotFound.razor                  #     Página 404
│   ├── Layout/                             #   🏗️ Componentes de layout
│   │   ├── MainLayout.razor                #     Layout principal (AppBar, footer, tema)
│   │   ├── MainLayout.razor.css            #     Estilos do layout
│   │   └── NavMenu.razor                   #     Menu de navegação lateral
│   ├── Services/                           #   🔌 Serviços (comunicação com API)
│   │   ├── AuthService.cs                  #     Autenticação, perfil, sessão
│   │   └── PlanoTreinoService.cs           #     CRUD de planos de treino
│   ├── wwwroot/                            #   🌐 Assets estáticos
│   │   ├── index.html                      #     Página host do Blazor
│   │   ├── css/                            #     Folhas de estilo
│   │   ├── img/                            #     Imagens (background, logos)
│   │   ├── img-dt/                         #     Imagens de dicas de treino
│   │   ├── img-gm/                         #     Imagens de ganho máximo
│   │   ├── js/                             #     Scripts JavaScript
│   │   └── sample-data/                    #     Dados de exemplo
│   └── Properties/                         #   ⚙️ Configurações
│       └── launchSettings.json             #     Perfis de execução
│
├── src/FitUP.WebApi/                       # ⚙️ Backend — ASP.NET Core Web API
│   ├── FitUP.WebApi.csproj                 #   Dependências: JWT, BCrypt, SqlClient, Swagger
│   ├── Program.cs                          #   Bootstrap, middleware pipeline, DI
│   ├── appsettings.json                    #   Configurações: connection string, JWT, CORS
│   ├── Properties/
│   │   └── launchSettings.json             #   Perfis de execução
│   ├── Controllers/                        #   🌐 Endpoints REST
│   │   ├── AuthController.cs               #     POST login, registrar, refresh-token
│   │   ├── UsuarioController.cs            #     GET/PUT/DELETE me (perfil, senha, desativar)
│   │   ├── PlanoTreinoController.cs        #     CRUD de planos de treino
│   │   ├── PlanoAlimentarController.cs     #     CRUD de planos alimentares
│   │   └── RegistroBioimpedanciaController.cs  # CRUD de registros de bioimpedância
│   ├── Service/                            #   🧠 Lógica de negócio
│   │   ├── IAuthService.cs / AuthService.cs           #   Login, registro, refresh token
│   │   ├── IUsuarioService.cs / UsuarioService.cs     #   CRUD usuário, alterar senha
│   │   ├── IPlanoTreinoService.cs / PlanoTreinoService.cs
│   │   ├── IPlanoAlimentarService.cs / PlanoAlimentarService.cs
│   │   ├── IRegistroBioimpedanciaService.cs / RegistroBioimpedanciaService.cs
│   │   └── ITokenService.cs / TokenService.cs         #   Geração de JWT + Refresh Token
│   ├── DTOs/                               #   📦 Objetos de Transferência
│   │   ├── LoginRequest.cs / RegistroRequest.cs
│   │   ├── AuthResponse.cs / RefreshTokenRequest.cs
│   │   ├── UsuarioDto.cs / UsuarioUpdateRequest.cs
│   │   ├── AlterarSenhaRequest.cs
│   │   ├── PlanoTreinoDto.cs / PlanoTreinoRequest.cs
│   │   ├── PlanoAlimentarDto.cs / PlanoAlimentarRequest.cs
│   │   └── RegistroBioimpedanciaDto.cs / RegistroBioimpedanciaRequest.cs
│   ├── Models/                             #   🗃️ Modelos de domínio
│   │   ├── Usuario.cs
│   │   ├── PlanoTreino.cs / DiaTreino.cs / Exercicio.cs
│   │   ├── PlanoAlimentar.cs / Refeicao.cs / Alimento.cs
│   │   └── RegistroBioimpedancia.cs
│   ├── Infrastructure/
│   │   └── script.sql                      #   📜 Script de criação do banco de dados
│   └── docs/                               #   📚 Documentação
│       ├── Arquitetura.md                  #     Visão geral da arquitetura
│       ├── visao.md                        #     Documento de visão do produto
│       ├── Roadmap.md                      #     Planejamento de releases
│       ├── Licoes-aprendidas.md            #     Retrospectiva técnica
│       ├── specs/                          #     Especificações funcionais
│       ├── ADR-0001-ado-net-sem-ef-core.md
│       ├── ADR-0002-identificador-guid-pk.md
│       ├── ADR-0003-soft-delete-usuario-hard-delete-planos.md
│       └── ADR-0004-banco-relacional.md
│
└── docs/                                   # 📚 Documentação adicional
```

---

## 🗄️ Banco de Dados

### Modelo Relacional

O banco de dados foi modelado seguindo os princípios de normalização (3FN), com chaves primárias usando **GUID** para evitar colisões em ambientes distribuídos e facilitar a geração de IDs no lado da aplicação.

### Principais Entidades

| Entidade | Descrição | Principais Colunas |
|----------|-----------|-------------------|
| **Usuario** | Dados do usuário e autenticação | `Id`, `Nome`, `Sobrenome`, `Email`, `SenhaHash`, `RefreshToken`, `Ativo` |
| **PlanoTreino** | Plano de treino criado pelo usuário | `Id`, `UsuarioId`, `Nome`, `Divisao`, `Objetivo`, `Nivel`, `FrequenciaSemanal` |
| **DiaTreino** | Dias específicos dentro de um plano | `Id`, `PlanoTreinoId`, `DiaSemana`, `Ordem` |
| **Exercicio** | Exercícios em cada dia de treino | `Id`, `DiaTreinoId`, `NomeExercicio`, `Series`, `Repeticoes`, `Carga` |
| **PlanoAlimentar** | Plano de alimentação do usuário | `Id`, `UsuarioId`, `Objetivo`, `DataInicio`, `DataFim` |
| **Refeicao** | Refeições dentro de um plano | `Id`, `PlanoAlimentarId`, `Tipo`, `Horario` |
| **Alimento** | Itens alimentares em cada refeição | `Id`, `RefeicaoId`, `Descricao`, `Calorias`, `Proteinas`, `Carboidratos`, `Gorduras` |
| **RegistroBioimpedancia** | Métricas corporais | `Id`, `UsuarioId`, `Peso`, `PercentualGordura`, `MassaMagra`, `DataMedicao` |

### Estratégias de Integridade e Performance

- **PKs como `UNIQUEIDENTIFIER` (GUID)** — Evita colisões, facilita geração client-side
- **Soft Delete em Usuario** — `Ativo = 0` em vez de `DELETE`, preservando histórico
- **Hard Delete em Planos** — Planos de treino e alimentares são removidos fisicamente
- **Consultas parametrizadas** — 100% das queries usam `SqlParameter` (proteção contra SQL Injection)
- **COALESCE em UPDATEs** — Campos `NULL` preservam o valor anterior, permitindo atualizações parciais

> Consulte `src/FitUP.WebApi/Infrastructure/script.sql` para o DDL completo.

---

## ⚡ Como Executar

### Pré-requisitos

| Ferramenta | Versão Mínima | Instalação |
|-----------|---------------|------------|
| **.NET SDK** | 10.0 | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/10.0) |
| **SQL Server** | 2019+ (ou LocalDB) | [microsoft.com/sql-server](https://www.microsoft.com/sql-server/) |
| **Git** | 2.x | [git-scm.com](https://git-scm.com/) |
| **IDE** (opcional) | VS 2022 / VS Code / Rider | — |

### 1. Clonar o Repositório

```bash
git clone https://github.com/Ianclrs/FitUP.git
cd FitUP
```

### 2. Configurar o Banco de Dados

1. Abra o **SQL Server Management Studio (SSMS)** ou **Azure Data Studio**
2. Conecte-se ao seu servidor SQL Server
3. Execute o script de criação do banco:
   ```bash
   sqlcmd -S localhost -i src/FitUP.WebApi/Infrastructure/script.sql
   ```
   Ou abra `src/FitUP.WebApi/Infrastructure/script.sql` no SSMS e execute (F5)
4. Atualize a string de conexão em `src/FitUP.WebApi/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=FitUP;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

### 3. Configurar JWT

Em `src/FitUP.WebApi/appsettings.json`, configure a seção `Jwt`:

```json
{
  "Jwt": {
    "Key": "SUA_CHAVE_SECRETA_COM_PELO_MENOS_32_CARACTERES_AQUI",
    "Issuer": "FitUP",
    "Audience": "FitUP",
    "ExpiraEmMinutos": 60
  }
}
```

> ⚠️ **Em produção**, armazene a chave JWT em variáveis de ambiente ou Azure Key Vault. **Nunca faça commit de chaves reais.**

### 4. Restaurar Dependências e Compilar

```bash
dotnet restore
dotnet build
```

### 5. Executar o Backend (API)

```bash
cd src/FitUP.WebApi
dotnet run
```

A API estará disponível em:
- `https://localhost:7001` (HTTPS)
- `http://localhost:7000` (HTTP)
- Swagger UI: `https://localhost:7001/swagger`

### 6. Executar o Frontend (Blazor)

Em um **segundo terminal**:

```bash
cd FitUP/FitUP
dotnet run
```

O frontend estará disponível em:
- `https://localhost:7211` (HTTPS)
- `http://localhost:5059` (HTTP)

### 7. Verificar o Funcionamento

1. Acesse `https://localhost:7211` no navegador
2. Clique em **Cadastrar** e crie uma conta
3. Faça **Login** com as credenciais criadas
4. Navegue até **Meu Perfil** para testar alteração de nome, e-mail e senha
5. Explore **Monte seu Treino**, **Ganho Máximo** e **Calculadora de Bioimpedância**

---

## 🔌 API Endpoints

### Autenticação (`/api/auth`)

| Método | Rota | Autenticação | Descrição |
|--------|------|--------------|-----------|
| `POST` | `/api/auth/registrar` | ❌ | Registrar novo usuário |
| `POST` | `/api/auth/login` | ❌ | Login — retorna JWT + Refresh Token |
| `POST` | `/api/auth/refresh-token` | ❌ | Renovar token JWT expirado |

### Usuário (`/api/usuario`)

| Método | Rota | Autenticação | Descrição |
|--------|------|--------------|-----------|
| `GET` | `/api/usuario/me` | ✅ Bearer | Obter dados do usuário logado |
| `PUT` | `/api/usuario/me` | ✅ Bearer | Atualizar perfil (nome, email, telefone, etc.) |
| `PUT` | `/api/usuario/me/alterar-senha` | ✅ Bearer | Alterar senha (requer senha atual) |
| `DELETE` | `/api/usuario/me` | ✅ Bearer | Desativar conta (soft delete) |

### Planos de Treino (`/api/planotreino`)

| Método | Rota | Autenticação | Descrição |
|--------|------|--------------|-----------|
| `GET` | `/api/planotreino` | ✅ Bearer | Listar planos do usuário |
| `GET` | `/api/planotreino/{id}` | ✅ Bearer | Obter plano por ID |
| `POST` | `/api/planotreino` | ✅ Bearer | Criar novo plano |
| `DELETE` | `/api/planotreino/{id}` | ✅ Bearer | Excluir plano |

### Planos Alimentares (`/api/planoalimentar`)

| Método | Rota | Autenticação | Descrição |
|--------|------|--------------|-----------|
| `GET` | `/api/planoalimentar` | ✅ Bearer | Listar planos do usuário |
| `GET` | `/api/planoalimentar/{id}` | ✅ Bearer | Obter plano por ID |
| `POST` | `/api/planoalimentar` | ✅ Bearer | Criar novo plano |
| `DELETE` | `/api/planoalimentar/{id}` | ✅ Bearer | Excluir plano |

### Bioimpedância (`/api/registrobioimpedancia`)

| Método | Rota | Autenticação | Descrição |
|--------|------|--------------|-----------|
| `GET` | `/api/registrobioimpedancia` | ✅ Bearer | Listar registros do usuário |
| `GET` | `/api/registrobioimpedancia/{id}` | ✅ Bearer | Obter registro por ID |
| `POST` | `/api/registrobioimpedancia` | ✅ Bearer | Registrar nova medição |
| `DELETE` | `/api/registrobioimpedancia/{id}` | ✅ Bearer | Excluir registro |

### Exemplo de Requisição — Alterar Senha

```http
PUT /api/usuario/me/alterar-senha HTTP/1.1
Host: localhost:7001
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

{
  "senhaAtual": "MinhaSenha123",
  "novaSenha": "NovaSenha@456",
  "confirmarNovaSenha": "NovaSenha@456"
}
```

**Resposta de sucesso (204 No Content)** — A senha foi alterada e o novo hash BCrypt foi persistido.

> Para explorar todos os endpoints interativamente, acesse o **Swagger UI** em `https://localhost:7001/swagger` com o backend rodando.

---

## 📐 Decisões de Arquitetura (ADR)

O projeto documenta decisões arquiteturais significativas no diretório `src/FitUP.WebApi/docs/`:

| ADR | Título | Decisão | Justificativa |
|-----|--------|---------|---------------|
| 0001 | **ADO.NET sem Entity Framework Core** | Usar `Microsoft.Data.SqlClient` com SQL raw | Maior controle sobre queries, performance previsível, sem camada de abstração desnecessária para o escopo do projeto |
| 0002 | **Identificador GUID como PK** | `UNIQUEIDENTIFIER` em todas as tabelas | Evita colisões em ambientes distribuídos, permite geração client-side, consistência entre domínios |
| 0003 | **Soft Delete para Usuário, Hard Delete para Planos** | `Ativo = 0` em Usuario; `DELETE` físico em planos | Preserva histórico e integridade referencial de usuários; planos são dados voláteis que não precisam de retenção |
| 0004 | **Banco Relacional (SQL Server)** | Modelagem normalizada (3FN) | Consistência transacional, integridade referencial, consultas complexas com JOINs, familiaridade da equipe |

---

## 🔒 Práticas de Segurança

| Prática | Implementação |
|---------|---------------|
| **Senhas** | Hash BCrypt com sal adaptativo (custo 12) — nunca armazenadas em texto plano |
| **Autenticação** | JWT com tempo de expiração configurável, Refresh Token com expiração de 7 dias |
| **Autorização** | Atributo `[Authorize]` nos controllers; extração do `UsuarioId` dos Claims do token |
| **SQL Injection** | 100% das queries usam `SqlParameter` (consultas parametrizadas) |
| **CORS** | Política restrita — apenas origens do frontend são permitidas |
| **HTTPS** | Redirecionamento HTTPS forçado em produção |
| **Senha atual** | Exigida para confirmação antes de qualquer alteração de senha |
| **Confirmação de e-mail** | Validação `[Compare]` — exige digitação dupla para troca de e-mail |

---

## 👥 Equipe

<table align="center">
  <tr>
    <td align="center">
      <a href="https://github.com/Ianclrs">
        <img src="https://avatars.githubusercontent.com/u/210450237?v=4"
             width="120" height="120"
             style="object-fit: cover; border-radius: 50%;"
             alt="Ian Carlos"/>
        <br/>
        <sub><b>Ian Carlos</b></sub>
        <br/>
        <sub>Desenvolvedor</sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/thauacerqueira">
        <img src="https://avatars.githubusercontent.com/u/161505109?v=4"
             width="120" height="120"
             style="object-fit: cover; border-radius: 50%;"
             alt="Thauã Cerqueira"/>
        <br/>
        <sub><b>Thauã Cerqueira</b></sub>
        <br/>
        <sub>Desenvolvedor</sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/FelipeTWD">
        <img src="https://avatars.githubusercontent.com/u/146496842?v=4"
             width="120" height="120"
             style="object-fit: cover; border-radius: 50%;"
             alt="Felipe Dario"/>
        <br/>
        <sub><b>Felipe Dario</b></sub>
        <br/>
        <sub>Scrum Master</sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/pdfreitass">
        <img src="https://avatars.githubusercontent.com/u/203004835?v=4"
             width="120" height="120"
             style="object-fit: cover; border-radius: 50%;"
             alt="Pedro Freitas"/>
        <br/>
        <sub><b>Pedro Freitas</b></sub>
        <br/>
        <sub>QA</sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/PedroHenriquePO">
        <img src="https://avatars.githubusercontent.com/u/247213111?v=4"
             width="120" height="120"
             style="object-fit: cover; border-radius: 50%;"
             alt="Pedro Henrique"/>
        <br/>
        <sub><b>Pedro Henrique</b></sub>
        <br/>
        <sub>Product Owner</sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/Castxr">
        <img src="https://avatars.githubusercontent.com/u/213353198?v=4"
             width="120" height="120"
             style="object-fit: cover; border-radius: 50%;"
             alt="Gabriel Castor"/>
        <br/>
        <sub><b>Gabriel Castor</b></sub>
        <br/>
        <sub>QA</sub>
      </a>
    </td>
  </tr>
</table>

---

## 📄 Licença

Este projeto está licenciado sob a **Licença MIT** — veja o arquivo [LICENSE](LICENSE) para detalhes.

---

<div align="center">
  <p>
    <strong>FitUP</strong> — Evoluindo com você! 💪
  </p>
  <p>
    <a href="https://github.com/Ianclrs/FitUP">GitHub</a> ·
    <a href="https://linktr.ee/MembrosFitUp">Instagram</a> ·
    <a href="https://linktr.ee/LinkedInMembros">LinkedIn</a>
  </p>
</div>
