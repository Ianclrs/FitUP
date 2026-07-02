# SPEC-040: CRUD de Planos Alimentares no front-end

**Prioridade:** Alta
**Dependências:** SPEC-020 (autenticação pronta)
**Status:** Não iniciado
**Progresso:** ░░░░░░░░░░░░░░░░░░░░ 0% (0/5)
**API utilizada:** `/api/planos-alimentares` (5 endpoints)

> **Alinhado com:** `visao.md` §3 (Planos alimentares), `Roadmap.md` Fase 2
> **Sub-specs:** se houver impedimento, criar SPEC-041, SPEC-042...

---

## Objetivo

Criar duas páginas no front-end para gerenciar planos alimentares: uma de listagem e uma de criação/edição.

## Página 1 — Listagem (`/meus-planos`)

**Componente:** `FitUP/Pages/MeusPlanos.razor`

### Layout
- Título: "Meus Planos Alimentares"
- Botão "Novo Plano" (coral) → navega para `/meus-planos/novo`
- Tabela MudBlazor com colunas:
  - Nome
  - Objetivo (Bulking, Cutting, Manutenção — mapear de int)
  - Ações: Editar (lápis) e Excluir (lixeira)

### Comportamento
- Ao carregar: `GET /api/planos-alimentares` → popular tabela
- Clicar Editar → `/meus-planos/{id}`
- Clicar Excluir → diálogo de confirmação → `DELETE /api/planos-alimentares/{id}` → recarregar
- Tabela vazia: "Nenhum plano alimentar criado ainda."

## Página 2 — Criação/Edição (`/meus-planos/novo` e `/meus-planos/{id}`)

**Componente:** `FitUP/Pages/MeusPlanosForm.razor`

### Layout
- Título: "Novo Plano Alimentar" ou "Editar Plano Alimentar"
- Formulário:
  - `MudTextField` Nome (required)
  - `MudSelect` Objetivo (Bulking=0, Cutting=1, Manutenção=2)
  - `MudTextField` Descrição (opcional, multiline)
- Botão "Salvar" (coral) → POST ou PUT
- Botão "Cancelar" → volta para `/meus-planos`

### Comportamento
- Modo criação: formulário vazio
- Modo edição: carrega dados com `GET /api/planos-alimentares/{id}`
- Salvar: Snackbar "Plano salvo!" → navegar para `/meus-planos`

## DTOs

**Request:**
```json
{
  "nome": "string",
  "objetivo": 0,
  "descricao": "string"
}
```

**Response:** `PlanoAlimentarDto` com `refeicoes` e `alimentos` aninhados. Exibição opcional nesta spec.

## Onde mexer

| Arquivo | Ação |
|---|---|
| `FitUP/Pages/MeusPlanos.razor` | **Novo.** Página de listagem |
| `FitUP/Pages/MeusPlanosForm.razor` | **Novo.** Página de formulário |
| `FitUP/Layout/NavMenu.razor` | Adicionar link "Meus Planos" |
| `FitUP/Services/PlanoAlimentarService.cs` | **Novo.** Serviço HTTP para `/api/planos-alimentares` |

## Critério de aceitação

- [ ] Listagem carrega planos do usuário
- [ ] Criar → aparece na listagem
- [ ] Editar → dados carregam → salva
- [ ] Excluir → confirmado → removido
- [ ] Não logado → redireciona para `/login`
