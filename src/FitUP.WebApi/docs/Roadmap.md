# FitUP — Roadmap

> **Guia de desenvolvimento sessão por sessão.**  
> Cada item tem prioridade, dependências e link para a especificação detalhada em `docs/specs/`.
> 
> **Progresso geral da Fase 2:** ░░░░░░░░░░░░░░░░░░░░ 0%

---

## Fase 1 — MVP (mínimo para funcionar)

**Progresso:** ████████████████████ 100%

| Prioridade | Status | Tarefa | Dependências | Spec |
|---|---|---|---|---|
| — | ✅ | Banco de dados: 8 tabelas, FKs, índices | SQL Server Express | `script.sql` |
| — | ✅ | Back-end: 5 Controllers, 22 endpoints, JWT, Swagger | Banco criado | — |
| — | ✅ | Front-end: 11 páginas estáticas/informativas | Projeto Blazor criado | — |
| — | ✅ | Tela de Login integrada com API | AuthController | — |
| — | ✅ | Tela de Cadastro integrada com API | AuthController | — |
| — | ✅ | Documento de visão (`visao.md`) | — | — |
| — | ✅ | Documento de arquitetura (`Arquitetura.md`) | — | — |

---

## Fase 2 — Funcional e seguro (em andamento)

**Progresso:** ░░░░░░░░░░░░░░░░░░░░ 0%

| Prioridade | Status | Tarefa | Dependências | Spec |
|---|---|---|---|---|
| 🔴 Alta | [ ] | Corrigir chave `ExpiresInMinutes` em `TokenService.cs` | Nenhuma | 🔧 1 linha |
| 🔴 Alta | [ ] | Remover `reader.Close()` manual no `AuthService.cs` | Nenhuma | 🔧 auto-contido |
| 🔴 Alta | [ ] | Adicionar validação server-side em `RegistrarAsync` | Nenhuma | [SPEC-010](specs/SPEC-010-validacao-server-side.md) |
| 🟡 Média | [ ] | Substituir `AddWithValue` por `Add` com tipo explícito | Nenhuma | 🔧 todos os Services |
| 🟡 Média | [ ] | Corrigir N+1 queries em listagens | Nenhuma | 🔧 2 Services |
| 🔴 Alta | [ ] | Armazenar JWT no localStorage + interceptor HTTP | SPEC-010, AuthController | [SPEC-020](specs/SPEC-020-autenticacao-frontend.md) |
| 🔴 Alta | [ ] | Criar `AuthStateProvider` + refresh token automático | SPEC-020 | [SPEC-020](specs/SPEC-020-autenticacao-frontend.md) |
| 🔴 Alta | [ ] | Tela de listagem de planos de treino (`/meus-treinos`) | SPEC-020 | [SPEC-030](specs/SPEC-030-crud-planos-treino.md) |
| 🔴 Alta | [ ] | Tela de criação/edição de plano de treino | Listagem pronta | [SPEC-030](specs/SPEC-030-crud-planos-treino.md) |
| 🔴 Alta | [ ] | Tela de listagem de planos alimentares (`/meus-planos`) | SPEC-020 | [SPEC-040](specs/SPEC-040-crud-planos-alimentares.md) |
| 🔴 Alta | [ ] | Tela de criação/edição de plano alimentar | Listagem pronta | [SPEC-040](specs/SPEC-040-crud-planos-alimentares.md) |
| 🔴 Alta | [ ] | Tela de histórico de bioimpedância (`/minha-evolucao`) | SPEC-020 | [SPEC-050](specs/SPEC-050-crud-bioimpedancia.md) |
| 🔴 Alta | [ ] | Tela de registro/edição de bioimpedância | Histórico pronto | [SPEC-050](specs/SPEC-050-crud-bioimpedancia.md) |
| 🟡 Média | [ ] | Menu de navegação condicional (login/logado) | SPEC-020 | — |
| 🟢 Baixa | [ ] | Adicionar projeto WebApi ao `FitUP.slnx` | Nenhuma | — |

---

## Fase 3 — Escala e futuro

**Progresso:** ░░░░░░░░░░░░░░░░░░░░ 0%

| Prioridade | Status | Tarefa | Dependências |
|---|---|---|---|
| 🟡 Média | [ ] | Dashboard com gráficos de evolução (MudChart) | CRUDs prontos |
| 🟡 Média | [ ] | Upload de foto de perfil | SPEC-020 |
| 🟢 Baixa | [ ] | Validações visuais e feedback (Snackbar com ícone) | CRUDs prontos |
| 🟢 Baixa | [ ] | Testes unitários nos Services (xUnit + Moq) | Bugs corrigidos |
| 🟢 Baixa | [ ] | Testes de integração nos Controllers | Testes unitários prontos |
| 🟢 Baixa | [ ] | Paginação nas listagens (`?page=1&pageSize=20`) | CRUDs prontos |
| 🟢 Baixa | [ ] | Logging estruturado (Serilog) | — |
| 🟢 Baixa | [ ] | Deploy back-end | Testes passando |
| 🟢 Baixa | [ ] | Deploy front-end | Integração completa |
| 🟢 Baixa | [ ] | CI/CD via GitHub Actions | Deploy definido |

---

## Regras do Roadmap

1. **Ordem de ataque:** de cima para baixo, prioridade Alta → Média → Baixa.
2. **Specs concluídas:** atualizar `Status` para ✅, recalcular barra de progresso.
3. **Impedimento em uma spec:** NÃO travar. Criar sub-spec (ex: SPEC-010 travou → SPEC-011) e continuar nas outras.
4. **Sub-spec resolvida:** voltar para a spec pai e concluir.
5. **Sempre alinhar com `visao.md` e `Arquitetura.md`** — nunca implementar algo que contradiga o que está documentado lá.

---

*Atualizado em 2026-06-27. Revisitar a cada sessão de desenvolvimento.*
