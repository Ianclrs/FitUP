# SPEC-030: CRUD de Planos de Treino no front-end

**Prioridade:** Alta
**Dependências:** SPEC-020 (autenticação pronta)
**Status:** Não iniciado
**Progresso:** ░░░░░░░░░░░░░░░░░░░░ 0% (0/6)
**API utilizada:** `/api/planos-treino` (5 endpoints)

> **Alinhado com:** `visao.md` §3 (Planos de treino), `Roadmap.md` Fase 2
> **Sub-specs:** se houver impedimento, criar SPEC-031, SPEC-032...

---

## Objetivo

Criar duas páginas no front-end para gerenciar planos de treino: uma de listagem e uma de criação/edição.

## Página 1 — Listagem (`/meus-treinos`)

**Componente:** `FitUP/Pages/MeusTreinos.razor`

### Layout
- Título: "Meus Treinos"
- Botão "Novo Plano" (coral, canto superior direito) → navega para `/meus-treinos/novo`
- Tabela MudBlazor (`MudTable`) com colunas:
  - Nome
  - Divisão (exibir texto: Upper/Lower, Fullbody, PPL, ABCD — mapear de int)
  - Nível (Iniciante, Intermediário, Avançado)
  - Dias/semana
  - Ações: botão Editar (ícone lápis) e botão Excluir (ícone lixeira)

### Comportamento
- Ao carregar: `GET /api/planos-treino` → popular tabela
- Clicar Editar → navegar para `/meus-treinos/{id}`
- Clicar Excluir → diálogo de confirmação → `DELETE /api/planos-treino/{id}` → recarregar lista
- Tabela vazia: exibir texto "Nenhum plano de treino criado ainda."

## Página 2 — Criação/Edição (`/meus-treinos/novo` e `/meus-treinos/{id}`)

**Componente:** `FitUP/Pages/MeusTreinosForm.razor`

### Layout
- Título: "Novo Plano" ou "Editar Plano" (depende se `{id}` está na rota)
- Formulário com:
  - `MudTextField` Nome (required)
  - `MudTextField` Descrição (opcional, multiline)
  - `MudSelect` Divisão (opções: Upper/Lower=0, Fullbody=1, PPL=2, ABCD=3)
  - `MudSelect` Nível (Iniciante=0, Intermediário=1, Avançado=2)
  - `MudNumericField` Frequência Semanal (1-7)
- Botão "Salvar" (coral) → `POST` ou `PUT` dependendo do modo
- Botão "Cancelar" → volta para `/meus-treinos`

### Comportamento
- Modo criação: rota `/meus-treinos/novo`, formulário vazio
- Modo edição: rota `/meus-treinos/{id:guid}`, carregar dados com `GET /api/planos-treino/{id}`, preencher formulário
- Ao salvar com sucesso: Snackbar "Plano salvo!" e navegar para `/meus-treinos`
- Erro: Snackbar com mensagem de erro

## DTOs (já existem no back-end)

Usar `System.Net.Http.Json` para serialização. O back-end espera:

**Request (POST/PUT):**
```json
{
  "nome": "string",
  "descricao": "string",
  "divisao": 0,
  "nivel": 0,
  "frequenciaSemanal": 3
}
```

**Response (GET):** `PlanoTreinoDto` com `dias` e `exercicios` aninhados. Para esta spec, exibir os dias/exercícios na listagem é opcional (pode ser um expand futuro).

## Onde mexer

| Arquivo | Ação |
|---|---|
| `FitUP/Pages/MeusTreinos.razor` | **Novo.** Página de listagem |
| `FitUP/Pages/MeusTreinosForm.razor` | **Novo.** Página de formulário |
| `FitUP/Layout/NavMenu.razor` | Adicionar link "Meus Treinos" (visível só quando autenticado) |
| `FitUP/Services/PlanoTreinoService.cs` | **Novo.** Serviço HTTP para `/api/planos-treino` |

## Critério de aceitação

- [ ] Listagem carrega planos do usuário logado
- [ ] Criar novo plano → aparece na listagem
- [ ] Editar plano existente → dados carregam no formulário → salva alterações
- [ ] Excluir plano → diálogo confirma → removido da listagem
- [ ] Usuário não logado é redirecionado para `/login`
- [ ] Responsivo (funciona em mobile)
