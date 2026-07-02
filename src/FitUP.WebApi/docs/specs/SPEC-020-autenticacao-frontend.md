# SPEC-020: Autenticação no front-end (JWT + Refresh Token)

**Prioridade:** Alta
**Dependências:** SPEC-010 (validação server-side corrigida), AuthController funcionando
**Status:** Não iniciado
**Progresso:** ░░░░░░░░░░░░░░░░░░░░ 0% (0/5)

> **Alinhado com:** `visao.md` §8 (API auth + JWT), `Roadmap.md` Fase 2
> **Sub-specs:** se houver impedimento, criar SPEC-021, SPEC-022...

---

## Objetivo

Fazer o front-end Blazor WASM gerenciar autenticação JWT de ponta a ponta: armazenar token após login, anexar a toda requisição autenticada, renovar automaticamente quando expirar, e prover estado de autenticação para os componentes.

## Requisitos

### R1 — Armazenar JWT no localStorage

Após login ou cadastro bem-sucedido, salvar no `localStorage`:
- `fitup_token`: o JWT
- `fitup_refresh_token`: o refresh token
- `fitup_token_expira_em`: `DateTime.UtcNow + 120 minutos`

### R2 — Interceptor HTTP

Criar um `DelegatingHandler` (`AuthMessageHandler`) que:
- Lê o token do `localStorage`
- Anexa header `Authorization: Bearer {token}` em toda requisição
- Se receber `401 Unauthorized`, tenta renovar com refresh token
- Se renovar falhar, limpa localStorage e redireciona para `/login`

### R3 — AuthStateProvider

Criar `FitUPAuthStateProvider : AuthenticationStateProvider` que:
- Lê o JWT do `localStorage`
- Decodifica os claims (NameIdentifier, Name, Email, GivenName)
- Expõe `AuthenticationState` para componentes via `[CascadingParameter]` e `AuthorizeView`
- Atualiza estado após login/logout

### R4 — Login e Cadastro atualizados

- Após sucesso, salvar tudo no localStorage e notificar `AuthStateProvider`
- Após logout, limpar localStorage e notificar `AuthStateProvider`

### R5 — Proteção de rotas

As páginas de CRUD (`/meus-treinos`, `/meus-planos`, `/minha-evolucao`) devem usar `[Authorize]` do Blazor e redirecionar para `/login` se não autenticado.

## Onde mexer

| Arquivo | Ação |
|---|---|
| `FitUP/Services/AuthService.cs` | Adicionar métodos `Logout()`, `IsAuthenticated()`, `GetToken()` |
| `FitUP/Services/AuthMessageHandler.cs` | **Novo.** DelegatingHandler interceptor |
| `FitUP/Services/FitUPAuthStateProvider.cs` | **Novo.** AuthenticationStateProvider |
| `FitUP/Pages/Login.razor` | Após login, salvar em localStorage + notificar auth state |
| `FitUP/Pages/Cadastro.razor` | Após cadastro, salvar em localStorage + notificar auth state |
| `FitUP/Program.cs` | Registrar AuthMessageHandler, AuthStateProvider, cascatear no HttpClient |
| `FitUP/Layout/MainLayout.razor` | Esconder botões Entrar/Cadastrar quando logado, mostrar nome + logout |

## Critério de aceitação

- [ ] Fazer login → token salvo no localStorage → recarregar página → continua logado
- [ ] Fazer login → acessar `/meus-treinos` → requisição vai com header Authorization
- [ ] Token expirado → refresh automático → requisição original refeita com sucesso
- [ ] Refresh token expirado → redirecionado para `/login`
- [ ] Clicar logout → localStorage limpo → botões Entrar/Cadastrar reaparecem
