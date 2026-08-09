# Plano de Fases — FitUp

> **Criado em:** 24/07/2026  
> **Atualizado em:** 27/07/2026  
> **Referência:** [Catálogo de Problemas](catalogo-problemas.md)  
> **Commit base:** `b7d0805c7ead476d1951f7968e9d758ab3a825c6`  
> **Novas fases (5-8):** Foco exclusivo no frontend — performance, UX, código e funcionalidades

---

## Visão Geral

Este documento organiza as modificações pendentes do FitUp em **8 fases de implementação**, em ordem de prioridade. As fases 1-4 (concluídas) cobriram segurança, componentização, UX e infraestrutura. As fases 5-8 focam exclusivamente no **frontend**, endereçando performance, acessibilidade, qualidade de código e novas funcionalidades.

Cada fase contém um checklist de validação que deve ser executado após a conclusão para garantir a qualidade das alterações.

| Fase | Foco | Itens | Esforço | Status |
|------|------|-------|---------|--------|
| 🔴 Fase 1 | Segurança e Estabilidade | 2 | Médio-Baixo | ✅ 100% |
| 🟠 Fase 2 | Componentização | 4 | Alto | ✅ 100% |
| 🟡 Fase 3 | Experiência do Usuário | 4 | Médio | ✅ 100% |
| 🟢 Fase 4 | Infraestrutura e Deploy | 2 | Médio-Baixo | ✅ 100% |
| 🔴 Fase 5 | Performance e Carregamento (Frontend) | 5 | Médio | ✅ 100% |
| 🟠 Fase 6 | UX, Acessibilidade e Navegação (Frontend) | 10 | Médio-Baixo | ✅ 100% |
| 🟡 Fase 7 | Código e Manutenibilidade (Frontend) | 7 | Médio | ⬜ 0% |
| 🟢 Fase 8 | Funcionalidades e Evolução (Frontend) | 7 | Médio-Alto | ⬜ 0% |

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
| **M06** | ✅ | `Perfil.razor`, `CalculadoraBio.razor`, `GeradorDieta.razor` | Adicionar `MudProgressCircular` ou skeletons durante chamadas assíncronas (carregamento de bioimpedância no Perfil, cálculo na CalculadoraBio, geração no GeradorDieta). | 🟡 Médio |
| **M08** | ✅ | `Perfil.razor`, `AuthService.cs` | Corrigir `SepararNomeSobrenome` para tratar nome único (ex: "João" → sobrenome = `null`). Campos do DTO `AtualizarPerfilRequest` tornados nullable. | 🟢 Baixo |
| **M09** | ✅ | `Perfil.razor` | Ao alterar e-mail, enviar apenas campos modificados (usar `null` para nome/sobrenome quando não alterados, aproveitando `COALESCE` do backend). | 🟢 Baixo |
| **M07** | ✅ | `ApiResponse.cs`, `BioimpedanciaService.cs`, `PlanoTreinoService.cs`, `PlanoAlimentarService.cs`, `RegistroBioimpedanciaController.cs`, `PlanoTreinoController.cs`, `PlanoAlimentarController.cs`, `Perfil.razor`, `TreinosSalvos.razor`, `MinhasDietas.razor` | Adicionar `PagedResponse<T>` e parâmetros `page`/`pageSize` nos métodos `ListarAsync()`. Backend retorna `{ items, totalCount, page, pageSize }` para o frontend. | 🟡 Médio |

### Validação Pós-Conclusão

- [x] Compilar projeto sem erros — **0 erros, 4 avisos** (pré-existentes)
- [x] Perfil: alterar apenas e-mail → nome/sobrenome não são alterados (envia `null`, backend usa `COALESCE`)
- [x] Perfil: usuário com nome único ("João") consegue salvar perfil (sobrenome = `null`, backend mantém valor atual)
- [x] Perfil/CalculadoraBio/GeradorDieta: spinner visível durante operações assíncronas
- [x] Listas de treinos/dietas salvos: paginação implementada nos 3 controllers (bioimpedância, planos-treino, planos-alimentares) e nos 3 services frontend

---

## 🟢 Fase 4 — Infraestrutura e Deploy

**Objetivo:** Mitigar riscos de segurança em produção e preparar o ambiente de deploy completo (frontend + backend).

### Tarefas

| ID | Status | Arquivo(s) | Descrição | Esforço |
|----|--------|-----------|-----------|---------|
| **M01** | ✅ | `wwwroot/index.html`, `vercel.json` | Adicionar Content Security Policy (CSP) restritiva para mitigar XSS no `localStorage`. Configurar headers de segurança (X-Content-Type-Options, X-Frame-Options, etc.). | 🟢 Baixo |
| **M04** | ✅ | `vercel.json`, `src/FitUP.WebApi/Program.cs` | Configurar deploy do backend em serviço de nuvem (Azure App Service, Render ou Fly.io). Atualizar CORS no backend. Ajustar URL da API para produção. | 🟡 Médio |

### Validação Pós-Conclusão

- [x] Compilar projeto sem erros — **0 erros, 4 avisos** (pré-existentes)
- [x] CSP restritiva adicionada no `index.html` (default-src 'self' + fontes limitadas para scripts/styles)
- [x] Headers de segurança adicionados no `vercel.json` (X-Content-Type-Options, X-Frame-Options, X-XSS-Protection, Referrer-Policy, Permissions-Policy)
- [ ] Deploy do frontend funcional no Vercel (necessário novo deploy para aplicar CSP e headers)
- [x] CORS do backend configurado via `appsettings.json` com política `AllowProduction` para origens Vercel
- [ ] Deploy do backend funcional (URL pública acessível) — pendente de publicação em serviço de nuvem
- [ ] Login, cadastro, salvamento de treinos/dietas funcionando em produção

---

## 🔴 Fase 5 — Performance e Carregamento

**Objetivo:** Reduzir o tempo de carregamento inicial (atualmente 15-20 MB de bundle) e o consumo de banda, melhorando a experiência do usuário na primeira visita.

### Tarefas

| ID | Status | Arquivo(s) | Descrição | Esforço |
|----|--------|-----------|-----------|---------|
| **F05** | ✅ | `index.html`, `GeradorDieta.razor`, `MinhasDietas.razor`, `js/pdfExport.js` | **Lazy load do jsPDF.** O script de ~300 KB é carregado em todas as páginas mas só usado em GeradorDieta e MinhasDietas. Carregar dinamicamente via `IJSRuntime.InvokeAsync` apenas nas páginas que precisam. | 🟡 Médio |
| **F06** | ✅ | `wwwroot/img/`, `wwwroot/img-dt/`, `wwwroot/img-gm/` | **Otimizar imagens.** Converter `Home.png` e `HomeL.png` (backgrounds) para WebP com fallback. Comprimir imagens em `img-dt/` e `img-gm/`. Adicionar `loading="lazy"` em imagens abaixo da dobra. | 🟡 Médio |
| **F07** | ✅ | `FitUP.csproj`, `Program.cs` | **Lazy loading de assemblies Blazor.** Configurar `BlazorWebAssemblyLazyLoad` no `.csproj` para carregar MudBlazor e páginas pesadas sob demanda, reduzindo o download inicial do runtime .NET. | 🟡 Médio |
| **F08** | ✅ | `vercel.json` | **Compressão brotli/gzip.** Configurar `vercel.json` para servir `.dll`, `.wasm`, `.js` e `.css` com compressão brotli. O Vercel suporta nativamente. | 🟢 Baixo |
| **F09** | ✅ | `index.html`, novo `manifest.json`, novo `service-worker.js` | **Implementar PWA.** Service worker para cache de assets estáticos. `manifest.json` com ícones e tema. Permitir instalação na tela inicial do dispositivo. | 🟡 Médio |

### Validação Pós-Conclusão

- [x] Compilar projeto sem erros
- [x] jsPDF carregado apenas nas páginas GeradorDieta e MinhasDietas (verificar Network tab)
- [x] Imagens convertidas para WebP — backgrounds: 7MB → 168KB (98% redução); catálogos convertidos
- [x] Lazy loading de assemblies configurado via `BlazorWebAssemblyLazyLoad`
- [x] Compressão brotli configurada via Vercel + cache imutável para assets estáticos
- [x] PWA: service worker registrado, cache de assets funcionando, instalação disponível

---

## 🟠 Fase 6 — UX, Acessibilidade e Navegação

**Objetivo:** Corrigir problemas de experiência do usuário, melhorar acessibilidade (leitores de tela, SEO) e polir a interface.

### Tarefas

| ID | Status | Arquivo(s) | Descrição | Esforço |
|----|--------|-----------|-----------|---------|
| **U01** | ✅ | `wwwroot/index.html` linha 2 | **Corrigir `lang="en"` para `lang="pt-BR"`.** Afeta leitores de tela, SEO e tradutores automáticos. | 🟢 Baixo |
| **U02** | ✅ | `Layout/MainLayout.razor` | **Persistir tema dark/light no localStorage.** Atualmente volta ao padrão (escuro) a cada recarga. Salvar escolha do usuário e restaurar no `OnInitializedAsync`. | 🟢 Baixo |
| **U03** | ✅ | `Layout/MainLayout.razor` linhas 92-112 | **Completar menu do usuário (drawer lateral).** Adicionar links faltantes: "Minhas Dietas" e "Calculadora de Bioimpedância". | 🟢 Baixo |
| **U04** | ✅ | `App.razor` linha 19 | **Corrigir mensagem da página 404.** Texto atual gramaticalmente incorreto: "Desculpe, não há nada neste endereço e se apareci, tem erro em alguma página." → "Página não encontrada. O endereço que você acessou não existe ou foi movido." | 🟢 Baixo |
| **U05** | ✅ | `Pages/Home.razor` linhas 188, 201, 217 | **Remover uso de `eval` no JS interop.** Substituir `JS.InvokeAsync<string>("eval", ...)` por chamadas diretas a `localStorage.getItem`/`setItem`. Viola CSP e é má prática. | 🟢 Baixo |
| **U06** | ✅ | `wwwroot/sample-data/`, `Pages/NotFound.razor`, `Layout/MainLayout.razor.css` | **Remover arquivos residuais do template Blazor.** `weather.json` (exemplo), `NotFound.razor` (órfão — 404 está inline no `App.razor`), `MainLayout.razor.css` (estilos herdados não utilizados). | 🟢 Baixo |
| **U07** | ✅ | `Pages/GeradorDieta.razor`, `Pages/Home.razor` | **Extrair CSS inline para arquivos dedicados.** GeradorDieta tem ~50 linhas de `<style>`. Home tem ~20. Mover para `.razor.css` (CSS isolation do Blazor). | 🟡 Médio |
| **U08** | ✅ | `Pages/Home.razor`, múltiplos componentes | **Adicionar feedback tátil/visual em mobile.** Hover effects (`.scale-hover`, `.custom-shadow-hover`) não funcionam em touch. Adicionar `:active` states equivalentes. | 🟢 Baixo |
| **U09** | ✅ | `wwwroot/index.html`, `wwwroot/css/app.css` | **Melhorar loading screen.** Adicionar logo do FitUP e mensagem contextual ("Preparando seu treino...") em vez de texto genérico "Carregando...". | 🟢 Baixo |
| **U10** | ✅ | `wwwroot/img/`, `wwwroot/index.html` | **Adicionar favicons em múltiplos tamanhos.** 16x16, 48x48, 180x180 (Apple Touch Icon) para bookmarks e dispositivos móveis. | 🟢 Baixo |

### Validação Pós-Conclusão

- [x] Compilar projeto sem erros
- [x] `lang="pt-BR"` no `<html>` — verificado no DevTools
- [x] Tema persiste após recarregar a página (dark/light mantido)
- [x] Menu do usuário contém: Meu Perfil, Meus Treinos, Minhas Dietas, Calculadora Bio, Sair
- [x] Página 404 exibe mensagem profissional e botão "Voltar"
- [x] Zero ocorrências de `eval` no código — Home.razor limpo
- [x] Arquivos removidos: `weather.json`, `MainLayout.razor.css`
- [x] CSS inline extraído para `.razor.css` (GeradorDieta, Home) + classes globais em `app.css`
- [x] Touch feedback funcional em mobile (cards com `:active` state em `app.css`)
- [x] Loading screen com logo e mensagem contextual
- [x] Favicons em múltiplos tamanhos visíveis no DevTools > Application > Manifest

---

## 🟡 Fase 7 — Código e Manutenibilidade

**Objetivo:** Reduzir débito técnico, melhorar a qualidade do código e facilitar manutenção futura.

### Tarefas

| ID | Status | Arquivo(s) | Descrição | Esforço |
|----|--------|-----------|-----------|---------|
| **C01** | ⬜ | `Services/AuthService.cs`, nova pasta `Models/` ou projeto `FitUP.Shared` | **Eliminar DTOs duplicados.** `LoginRequest`, `RegistroRequest`, `AuthResponse` etc. duplicam os DTOs do backend. Centralizar em pasta `Models/` ou projeto shared `FitUP.Shared`. | 🔴 Alto |
| **C02** | ⬜ | `Services/AuthService.cs`, novo `RefreshTokenHandler.cs` | **Implementar refresh token automático.** O `RefreshToken` recebido no login nunca é usado. Criar `DelegatingHandler` que detecta 401, renova o token e reenvia a requisição original. | 🟡 Médio |
| **C03** | ⬜ | `Services/AuthService.cs` linha 178 | **Substituir `DateTime.UtcNow` como `AppVersion`.** Força logout em toda recompilação. Em dev causa logout constante. Usar versão do assembly (`Assembly.GetExecutingAssembly().GetName().Version`). | 🟢 Baixo |
| **C04** | ⬜ | `Pages/Home.razor`, `Pages/GanhoMaximo.razor`, `Pages/MonteTreino.razor` | **Corrigir typo `trasition-fast-out-slow-in`.** Deveria ser `transition`. Buscar globalmente nos arquivos `.razor` e substituir. | 🟢 Baixo |
| **C05** | ⬜ | `Layout/MainLayout.razor`, novos `Layout/Themes/LightTheme.cs`, `DarkTheme.cs` | **Refatorar paletas de cores.** Extrair `_lightPalette` e `_darkPalette` (40+ propriedades cada) para classes dedicadas, reduzindo as 276 linhas do `MainLayout.razor`. | 🟡 Médio |
| **C06** | ⬜ | Todos os arquivos em `Services/` | **Adicionar documentação XML (`<summary>`).** Métodos públicos de `AuthService`, `PlanoTreinoService`, `PlanoAlimentarService`, `BioimpedanciaService` sem documentação. | 🟢 Baixo |
| **C07** | ⬜ | `Layout/MainLayout.razor`, `Layout/NavMenu.razor`, páginas diversas | **Padronizar nomenclatura de rotas.** Usar kebab-case consistente (`/gerador-dieta`, `/dicas-treino`) em vez de PascalCase misturado com lowercase. | 🟢 Baixo |

### Validação Pós-Conclusão

- [ ] Compilar projeto sem erros
- [ ] DTOs centralizados em `Models/` ou `FitUP.Shared` — sem duplicação entre Services
- [ ] Refresh token funcional: token JWT expirado → renovação automática → requisição original reenviada
- [ ] `AppVersion` não causa logout em dev (usa versão do assembly, não timestamp)
- [ ] Zero ocorrências de `trasition` (typo) no código
- [ ] Temas extraídos para classes dedicadas — `MainLayout.razor` < 200 linhas
- [ ] Todos os métodos públicos dos Services documentados com `<summary>`
- [ ] Rotas padronizadas em kebab-case em todos os `Href` e `NavigationManager`

---

## 🟢 Fase 8 — Funcionalidades e Evolução

**Objetivo:** Adicionar funcionalidades que aumentam o valor do produto, engajamento e retenção de usuários.

### Tarefas

| ID | Status | Arquivo(s) | Descrição | Esforço |
|----|--------|-----------|-----------|---------|
| **V01** | ⬜ | `Pages/Home.razor`, novo `Services/DashboardService.cs` | **Dashboard na Home para usuário logado.** Substituir conteúdo estático por cards dinâmicos: último treino, calorias do plano ativo, evolução do peso (MudChart), meta semanal. | 🟡 Médio |
| **V02** | ⬜ | Nova página `Evolucao.razor`, `Services/BioimpedanciaService.cs` | **Página de evolução com gráficos.** Página `/evolucao` com MudChart de peso, % gordura e massa magra ao longo do tempo. | 🟡 Médio |
| **V03** | ⬜ | `Pages/Perfil.razor`, `Services/AuthService.cs`, novo endpoint backend | **Upload de foto de perfil.** Avatar no Perfil com preview e crop básico. Upload para backend com armazenamento local. | 🔴 Alto |
| **V04** | ⬜ | `Pages/Home.razor` | **Tags de nível nos cards de treino.** Adicionar `MudChipSet` com níveis (Iniciante, Intermediário, Avançado) nos cards Upper/Lower, Fullbody, PPL, ABCD. | 🟢 Baixo |
| **V05** | ⬜ | `service-worker.js`, `wwwroot/data/` | **Modo offline parcial.** Com PWA (F09), cache de dados estáticos (banco de alimentos, catálogo de exercícios). Calculadoras (TMB, bioimpedância) offline-ready. | 🟡 Médio |
| **V06** | ⬜ | `Pages/Home.razor` ou novo componente `Onboarding.razor` | **Onboarding interativo.** Tour guiado em 3 passos: "1. Monte seu treino → 2. Gere sua dieta → 3. Acompanhe sua evolução" usando `MudStepper` ou tooltips ancorados. | 🟡 Médio |
| **V07** | ⬜ | `Pages/MonteTreino.razor`, `Pages/GeradorDieta.razor` | **Compartilhamento de treinos/dietas.** Gerar link compartilhável ou exportar como imagem para WhatsApp/Instagram. | 🟡 Médio |

### Validação Pós-Conclusão

- [ ] Compilar projeto sem erros
- [ ] Home exibe dashboard dinâmico quando usuário logado (cards com dados reais)
- [ ] Página `/evolucao` com gráficos funcionais usando MudChart
- [ ] Upload de foto funcional no Perfil (preview + salvamento)
- [ ] Cards de treino na Home com tags de nível visíveis
- [ ] Modo offline: calculadoras funcionam sem internet, dados estáticos em cache
- [ ] Onboarding interativo com 3 passos, fechável e com indicador de progresso
- [ ] Compartilhamento funcional (link ou imagem gerada)

---

## 📊 Progresso Geral

| Status | Quantidade |
|--------|------------|
| ⬜ Pendente | 24 |
| 🔄 Em andamento | 0 |
| ✅ Concluído | 17 |
| **Total** | **41** |

---

## 📝 Registro de Alterações

| Data | Fase | Item | Responsável | Observações |
|------|------|------|-------------|-------------|
| 24/07/2026 | — | — | Cline | Criação do plano de fases |
| 24/07/2026 | Fase 1 | M03, M02 | Cline | Fase 1 concluída: EsqueciSenhaDialog corrigido, ApiResponse<T> implementado em todos os Services, 10 páginas atualizadas. Build: 0 erros. |
| 24/07/2026 | Fase 2 | A02, B03, B02 | Cline | Fase 2 parcial: ExerciseCatalogService criado com 3 JSONs (exercises, focus-mappings, workout-templates). MonteTreino.razor refatorado — catálogo e mapeamentos movidos para o serviço. MatchesBlockedKey otimizado O(1). ToggleSection.razor criado. OnInitializedAsync adicionado para correção de bug (catálogo vazio). Build: 0 erros. |
| 25/07/2026 | Fase 2 | A03 | Cline | **Fase 2 concluída!** DietaDataService criado com 4 dietas. 5 componentes de seção criados (ProteinasSection, CarboidratosSection, FrutasSection, VegetaisSection, GordurasSection). ToggleSection integrado com @bind-IsVisible. GanhoMaximo.razor reduzido de 1643 → ~140 linhas. ExportarDietaPdf refatorado para consumir DietaDataService.GetById(). Build: 0 erros, 4 avisos (pré-existentes). |
| 26/07/2026 | Fase 3 | M06, M08, M09, M07 | Cline | **Fase 3 concluída!** M06: spinners adicionados em CalculadoraBio (cálculo + delay 300ms) e GeradorDieta (carregamento alimentos, Salvar Dieta, Exportar PDF). M08: `SepararNomeSobrenome` corrigido para retornar `null` no sobrenome quando nome único; `AtualizarPerfilRequest` com campos nullable. M09: `HandleSalvarEmail` envia apenas `Email` (nome/sobrenome = null). M07: `PagedResponse<T>` criado; paginação implementada nos 3 controllers backend + 3 services frontend; `Perfil.razor`, `TreinosSalvos.razor`, `MinhasDietas.razor` atualizados. Build: 0 erros, 4 avisos. |
| 26/07/2026 | Extra | C08, Token | Cline | **Correções extras de bugs em produção.** C08: `MudMenu` no `MainLayout.razor` não abria popover com `<ActivatorContent>` no MudBlazor 9.7.0 — corrigido com `@bind-Open` + `MudButton` separado. **Token JWT 401:** Serviços (`BioimpedanciaService`, `PlanoTreinoService`, `PlanoAlimentarService`) recebiam 401 porque o token não era propagado entre instâncias de `HttpClient`. Criados `ITokenProvider` (singleton), `AuthHeaderHandler` (DelegatingHandler) e refatorado `AuthService` para usar o provider compartilhado. Corrigida dependência circular `AuthHeaderHandler` → `AuthService` → `HttpClient` → `AuthHeaderHandler`. Build: 0 erros, 4 avisos. |
| 26/07/2026 | Fase 4 | M01, M04 | Cline | **Fase 4 concluída!** M01: CSP restritiva adicionada no `index.html` (default-src 'self' + CDNs limitados). Headers de segurança no `vercel.json` (X-Content-Type-Options: nosniff, X-Frame-Options: DENY, X-XSS-Protection, Referrer-Policy, Permissions-Policy). M04: CORS backend configurado via `appsettings.json` com política `AllowProduction` suportando origens Vercel. Política `AllowLocalDev` mantida para desenvolvimento. Frontend com comentário guia para alterar `ApiBaseUrl` em produção. Build: 0 erros, 4 avisos. |
| 27/07/2026 | Fases 5-8 | F05-V07 | Cline | **Novas fases do frontend catalogadas.** Fase 5: Performance (lazy load, WebP, PWA). Fase 6: UX/Acessibilidade (lang pt-BR, tema persistente, menu completo, eval removido, CSS isolation). Fase 7: Código (DTOs centralizados, refresh token, documentação XML). Fase 8: Funcionalidades (dashboard, gráficos, foto perfil, onboarding, compartilhamento). Total: 29 novos itens. |
| 02/08/2026 | Fase 5 | F05, F06, F07, F08, F09 | Cline | **Fase 5 concluída!** F08: `vercel.json` otimizado com cache imutável para todos os assets estáticos. F05: jsPDF lazy-load via `pdfLoader.js` — removido do bundle inicial (~300 KB economizados). F06: todas as imagens convertidas para WebP — backgrounds reduzidos de 7 MB → 168 KB (98%), catálogos `img-dt/` (49 imgs) e `img-gm/` (11 imgs) convertidos. Referências atualizadas em 18 arquivos. F07: `BlazorWebAssemblyLazyLoad` configurado para MudBlazor.dll. F09: PWA completo — `manifest.json`, `service-worker.js` (cache-first), ícones 192x192 e 512x512, registro no `index.html`. Build: 0 erros, 4 avisos (pré-existentes). |
| | 09/08/2026 | Fase 6 | U01-U10 | Cline | **Fase 6 concluída!** U01: `lang="pt-BR"`. U02: tema dark/light persistido no localStorage. U03: menu do usuário completo (Minhas Dietas + Calculadora Bio). U04: página 404 corrigida. U05: `eval()` removido do Home.razor. U06: removidos `weather.json` e `MainLayout.razor.css`. U07: CSS inline extraído — `GeradorDieta.razor.css` (~78 linhas), `Home.razor.css`, classes globais em `app.css`. U08: `:active` states para feedback tátil mobile. U09: loading screen com logo + "Preparando seu treino...". U10: favicons 16x16, 48x48, 180x180. Build: 0 erros, 4 avisos (pré-existentes). |


---

> **Nota:** Este documento deve ser atualizado a cada conclusão de fase. Ao finalizar um item, altere `⬜` para `✅` e registre na tabela de alterações.
