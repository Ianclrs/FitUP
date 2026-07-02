# SPEC-050: CRUD de Bioimpedância no front-end

**Prioridade:** Alta
**Dependências:** SPEC-020 (autenticação pronta)
**Status:** Não iniciado
**Progresso:** ░░░░░░░░░░░░░░░░░░░░ 0% (0/7)
**API utilizada:** `/api/bioimpedancia` (5 endpoints)

> **Alinhado com:** `visao.md` §3 (Bioimpedância), `Roadmap.md` Fase 2
> **Sub-specs:** se houver impedimento, criar SPEC-051, SPEC-052...

---

## Objetivo

Criar duas páginas no front-end para registrar e visualizar medições de bioimpedância.

## Página 1 — Histórico (`/minha-evolucao`)

**Componente:** `FitUP/Pages/MinhaEvolucao.razor`

### Layout
- Título: "Minha Evolução"
- Botão "Novo Registro" (coral) → `/minha-evolucao/novo`
- Tabela MudBlazor com colunas:
  - Data (formato dd/MM/yyyy)
  - Peso (kg)
  - Altura (cm)
  - IMC (calculado: peso / (altura/100)²)
  - % Gordura
  - Ações: Editar (lápis) e Excluir (lixeira)

### Comportamento
- Ao carregar: `GET /api/bioimpedancia` → popular tabela (ordenada por data decrescente)
- Clicar Editar → `/minha-evolucao/{id}`
- Clicar Excluir → diálogo → `DELETE /api/bioimpedancia/{id}` → recarregar
- Tabela vazia: "Nenhum registro de bioimpedância ainda."

## Página 2 — Registro/Edição (`/minha-evolucao/novo` e `/minha-evolucao/{id}`)

**Componente:** `FitUP/Pages/MinhaEvolucaoForm.razor`

### Layout
- Título: "Novo Registro" ou "Editar Registro"
- Formulário em duas colunas (`MudGrid`):

  **Coluna 1 — Essenciais:**
  - `MudDatePicker` Data do Registro (required, default hoje)
  - `MudNumericField` Peso (kg, required, step 0.1)
  - `MudNumericField` Altura (cm, required)

  **Coluna 2 — Composição Corporal:**
  - `MudNumericField` Massa Magra (kg, opcional)
  - `MudNumericField` Massa Gorda (kg, opcional)
  - `MudNumericField` % Gordura (opcional)
  - `MudNumericField` Massa Muscular (kg, opcional)

  **Coluna 3 — Métricas Avançadas:**
  - `MudNumericField` Água Corporal (L, opcional)
  - `MudNumericField` TMB (kcal, opcional)
  - `MudNumericField` Idade Metabólica (opcional)

  **Coluna 4 — Medidas:**
  - `MudNumericField` Cintura (cm, opcional)
  - `MudNumericField` Quadril (cm, opcional)
  - `MudNumericField` Relação Cintura/Quadril (opcional)

  **Full width:**
  - `MudTextField` Observações (opcional, multiline)

- Botão "Salvar" (coral) → POST ou PUT
- Botão "Cancelar" → volta para `/minha-evolucao`

### Comportamento
- Modo criação: data default = hoje, resto vazio
- Modo edição: `GET /api/bioimpedancia/{id}`, preencher todos os campos
- Campos opcionais que ficarem vazios: enviar como `null` (não 0)

## DTOs

**Request:**
```json
{
  "dataRegistro": "2026-06-27T00:00:00",
  "peso": 80.5,
  "altura": 175.0,
  "massaMagra": null,
  "massaGorda": null,
  "percentualGordura": 15.2,
  "massaMuscular": null,
  "aguaCorporal": null,
  "taxaMetabolicaBasal": null,
  "idadeMetabolica": null,
  "circunferenciaCintura": null,
  "circunferenciaQuadril": null,
  "relacaoCinturaQuadril": null,
  "observacoes": null
}
```

## Onde mexer

| Arquivo | Ação |
|---|---|
| `FitUP/Pages/MinhaEvolucao.razor` | **Novo.** Página de histórico |
| `FitUP/Pages/MinhaEvolucaoForm.razor` | **Novo.** Página de formulário |
| `FitUP/Layout/NavMenu.razor` | Adicionar link "Minha Evolução" |
| `FitUP/Services/BioimpedanciaService.cs` | **Novo.** Serviço HTTP para `/api/bioimpedancia` |

## Critério de aceitação

- [ ] Histórico carrega registros do usuário, ordenados por data
- [ ] Criar novo registro com dados essenciais → aparece no histórico
- [ ] Criar registro com todos os campos opcionais → todos salvos corretamente
- [ ] Editar registro → dados carregam → salva
- [ ] Excluir → confirmado → removido
- [ ] IMC calculado corretamente na tabela do histórico
- [ ] Não logado → redireciona para `/login`
