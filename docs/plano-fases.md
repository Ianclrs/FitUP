# Plano de Fases — FitUp

> **Criado em:** 24/07/2026  
> **Referência:** [Catálogo de Problemas](catalogo-problemas.md)  
> **Commit base:** `b7d0805c7ead476d1951f7968e9d758ab3a825c6`

---

## Visão Geral

Este documento organiza as modificações pendentes do FitUp em **4 fases de implementação**, em ordem de prioridade. Cada fase contém um checklist de validação que deve ser executado após a conclusão para garantir a qualidade das alterações.

| Fase | Foco | Itens | Esforço |
|------|------|-------|---------|
| 🔴 Fase 1 | Segurança e Estabilidade | 2 | Médio-Baixo |
| 🟠 Fase 2 | Componentização | 4 | Alto |
| 🟡 Fase 3 | Experiência do Usuário | 4 | Médio |
| 🟢 Fase 4 | Infraestrutura e Deploy | 2 | Médio-Baixo |

---

## 🔴 Fase 1 — Segurança e Estabilidade

**Objetivo:** Corrigir vulnerabilidades de segurança e estabelecer uma base sólida de tratamento de erros que permita feedback adequado ao usuário nas fases seguintes.

### Tarefas

| ID | Status | Arquivo(s) | Descrição | Esforço |
|----|--------|-----------|-----------|---------|
| **M03** | ✅ | `FitUP/Pages/EsqueciSenhaDialog.razor` | Remover alerta de dev que expõe link de reset na tela e copia para clipboard. O link de reset jamais deve ser acessível pelo frontend. | 🟢 Baixo |
| **M02** | ✅ | `FitUP/Services/AuthService.cs`, `PlanoTreinoService.cs`, `PlanoAlimentarService.cs`, `BioimpedanciaService.cs` | Implementar classe `ApiResponse<T>` com `Success`, `ErrorMessage`, `StatusCode`. Substituir retornos `null`/`false`/lista vazia por `ApiResponse<T>`. Adicionar tratamento por status code (401 → redirecionar login, 500 → mensagem genérica). | 🟡 Médio |

### Validação Pós-Conclusão

- [x] Compilar projeto sem erros: `dotnet build` — **0 erros, 4 avisos** (todos pré-existentes)
- [x] **M03:** Removido alerta de dev e exposição do link — agora exibe apenas mensagem genérica de e-mail enviado
- [x] **M02:** Todos os 4 Services retornam `ApiResponse<T>` com tratamento por status code (400, 401, 404, 500, etc.)
- [x] **M02:** Adicionado `catch` para `HttpRequestException` (falha de rede) e `TaskCanceledException` (timeout)
- [x] **M02:** Todas as páginas consumidoras (Login, Cadastro, Perfil, RedefinirSenha, EsqueciSenha, MonteTreino, TreinosSalvos, GeradorDieta, MinhasDietas, CalculadoraBio) atualizadas para usar `result.Success` / `result.ErrorMessage`

---

## 🟠 Fase 2 — Componentização

**Objetivo:** Reduzir o tamanho dos arquivos gigantes (MonteTreino: 1459 linhas, GanhoMaximo: 1643 linhas) extraindo dados hardcoded e criando componentes reutilizáveis.

### Tarefas

| ID | Status | Arquivo(s) | Descrição | Esforço |
|----|--------|-----------|-----------|---------|
| **A02** | ✅ | `MonteTreino.razor` → `Services/ExerciseCatalogService.cs`, `wwwroot/data/exercises.json` | Extrair catálogo de 150+ exercícios, templates de workout e mapeamentos de foco para serviços e arquivos JSON. | 🔴 Alto |
| **B03** | ✅ | `MonteTreino.razor` | Substituir `MatchesBlockedKey` O(n²) por `Dictionary<string, string>` reverso mapeando nome do exercício → chave do catálogo. | 🟢 Baixo |
| **A03** | ✅ | `GanhoMaximo.razor` → `Services/DietaDataService.cs`, `Components/GanhoMaximo/` | Extrair 4 dietas completas para serviço de dados. Componentizar cada seção (ProteinasSection, CarboidratosSection, FrutasSection, VegetaisSection, GordurasSection). | 🔴 Alto |
| **B02** | ✅ | `GanhoMaximo.razor` → `Components/GanhoMaximo/ToggleSection.razor` | Criar componente reutilizável para os 5 toggles idênticos (Proteínas, Carboidratos, Frutas, Vegetais, Gorduras). | 🟡 Médio |

### Validação Pós-Conclusão

- [x] Compilar projeto sem erros — **0 erros, 4 avisos** (pré-existentes)
- [x] MonteTreino: fluxo completo (selecionar divisão → nível → objetivo → frequência → gerar treino → salvar) funcionando
- [x] MonteTreino: catálogo de exercícios movido para `ExerciseCatalogService` + arquivos JSON em `wwwroot/data/`
- [x] MonteTreino: `MatchesBlockedKey` otimizado de O(n²) para O(1) via `ExerciseNameToKey` (dicionário reverso)
- [x] Componente `ToggleSection.razor` criado em `Components/GanhoMaximo/` para reutilização nos toggles
- [x] GanhoMaximo: refatoração concluída — `GanhoMaximo.razor` reduzido de 1643 → ~140 linhas usando `ToggleSection`, 5 componentes de seção e `DietaDataService`
- [x] GanhoMaximo: exportação PDF refatorada para consumir `DietaDataService.GetById()` — mantém mesma estrutura JSON
- [x] Arquivos JSON externos são carregados corretamente (sem 404) com fallback hardcoded no serviço

---

## 🟡 Fase 3 — Experiência do Usuário

**Objetivo:** Melhorar o feedback visual durante carregamentos, corrigir bugs de UX no Perfil e implementar paginação para escala.

### Tarefas

| ID | Status | Arquivo(s) | Descrição | Esforço |
|----|--------|-----------|-----------|---------|
| **M06** | ⬜ | `Perfil.razor`, `CalculadoraBio.razor`, `GeradorDieta.razor` | Adicionar `MudProgressCircular` ou skeletons durante chamadas assíncronas (carregamento de bioimpedância no Perfil, cálculo na CalculadoraBio, geração no GeradorDieta). | 🟡 Médio |
| **M08** | ⬜ | `Perfil.razor` | Corrigir `SepararNomeSobrenome` para tratar nome único (ex: "João" → sobrenome = "" ou repetir nome). | 🟢 Baixo |
| **M09** | ⬜ | `Perfil.razor` | Ao alterar e-mail, enviar apenas campos modificados (usar `null` para nome/sobrenome quando não alterados, aproveitando `COALESCE` do backend). | 🟢 Baixo |
| **M07** | ⬜ | Todos os Services (frontend + backend) | Adicionar parâmetros `page`/`pageSize` nos métodos `ListarAsync()`. Backend deve retornar total de registros para o frontend calcular número de páginas. | 🟡 Médio |

### Validação Pós-Conclusão

- [ ] Compilar projeto sem erros
- [ ] Perfil: alterar apenas e-mail → nome/sobrenome não são alterados
- [ ] Perfil: usuário com nome único ("João") consegue salvar perfil
- [ ] Perfil/CalculadoraBio/GeradorDieta: spinner visível durante operações assíncronas
- [ ] Listas de treinos/dietas salvos: paginação funcionando com navegação entre páginas

---

## 🟢 Fase 4 — Infraestrutura e Deploy

**Objetivo:** Mitigar riscos de segurança em produção e preparar o ambiente de deploy completo (frontend + backend).

### Tarefas

| ID | Status | Arquivo(s) | Descrição | Esforço |
|----|--------|-----------|-----------|---------|
| **M01** | ⬜ | `wwwroot/index.html`, `vercel.json` | Adicionar Content Security Policy (CSP) restritiva para mitigar XSS no `localStorage`. Configurar headers de segurança (X-Content-Type-Options, X-Frame-Options, etc.). | 🟢 Baixo |
| **M04** | ⬜ | `vercel.json`, `src/FitUP.WebApi/Program.cs` | Configurar deploy do backend em serviço de nuvem (Azure App Service, Render ou Fly.io). Atualizar CORS no backend. Ajustar URL da API para produção. | 🟡 Médio |

### Validação Pós-Conclusão

- [ ] Compilar projeto sem erros
- [ ] Deploy do frontend funcional no Vercel
- [ ] Deploy do backend funcional (URL pública acessível)
- [ ] Login, cadastro, salvamento de treinos/dietas funcionando em produção
- [ ] Headers de segurança presentes nas respostas HTTP

---

## 📊 Progresso Geral

| Status | Quantidade |
|--------|------------|
| ⬜ Pendente | 6 |
| 🔄 Em andamento | 0 |
| ✅ Concluído | 6 |

---

## 📝 Registro de Alterações

| Data | Fase | Item | Responsável | Observações |
|------|------|------|-------------|-------------|
| 24/07/2026 | — | — | Cline | Criação do plano de fases |
| 24/07/2026 | Fase 1 | M03, M02 | Cline | Fase 1 concluída: EsqueciSenhaDialog corrigido, ApiResponse<T> implementado em todos os Services, 10 páginas atualizadas. Build: 0 erros. |
| 24/07/2026 | Fase 2 | A02, B03, B02 | Cline | Fase 2 parcial: ExerciseCatalogService criado com 3 JSONs (exercises, focus-mappings, workout-templates). MonteTreino.razor refatorado — catálogo e mapeamentos movidos para o serviço. MatchesBlockedKey otimizado O(1). ToggleSection.razor criado. OnInitializedAsync adicionado para correção de bug (catálogo vazio). Build: 0 erros. |
| 25/07/2026 | Fase 2 | A03 | Cline | **Fase 2 concluída!** DietaDataService criado com 4 dietas. 5 componentes de seção criados (ProteinasSection, CarboidratosSection, FrutasSection, VegetaisSection, GordurasSection). ToggleSection integrado com @bind-IsVisible. GanhoMaximo.razor reduzido de 1643 → ~140 linhas. ExportarDietaPdf refatorado para consumir DietaDataService.GetById(). Build: 0 erros, 4 avisos (pré-existentes). |

---

> **Nota:** Este documento deve ser atualizado a cada conclusão de fase. Ao finalizar um item, altere `⬜` para `✅` e registre na tabela de alterações.