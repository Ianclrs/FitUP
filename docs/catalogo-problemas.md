# Catálogo de Problemas — FitUP

> **Última atualização:** 24/07/2026  
> **Commit analisado:** `b7d0805c7ead476d1951f7968e9d758ab3a825c6`  
> **Versão do projeto:** .NET 10.0 / Blazor WebAssembly / MudBlazor 9.x  
> **Analisado por:** Cline (revisão completa do código)  
> **Fase 1 concluída:** M02 (ApiResponse) e M03 (EsqueciSenha) resolvidos

---

## 📊 Resumo

| Severidade | Quantidade |
|-----------|------------|
| 🔴 Crítico | 8 |
| 🟠 Alto | 7 |
| 🟡 Médio | 10 |
| 🟢 Baixo | 5 |
| **Total** | **30** |

---

## 🔴 Problemas Críticos

| ID | Status | Arquivo | Linha | Problema | Sugestão de Correção |
|----|--------|---------|-------|----------|----------------------|
| C01 | ✅ | `src/FitUP.WebApi/` | — | **~~Backend ausente~~ Falso positivo.** O backend está presente e completo: 5 Controllers, 15 DTOs, 7 Models, 10 Services, script SQL e documentação. Compila com 0 erros. | Nenhuma ação necessária. |
| C02 | ✅ | `FitUP/Program.cs` | 12 | **URL da API hardcoded** como `http://localhost:5000`. Não usa HTTPS, não lê de `appsettings.json`, não suporta múltiplos ambientes. Em produção (Vercel), essa URL será inalcançável. | Movido para `wwwroot/appsettings.json` com fallback: `builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5000"`. |
| C03 | ✅ | `FitUP/Layout/MainLayout.razor` | 92 | **CSS quebrado:** `background: url('...'); background: cover;` — a segunda propriedade sobrescreve a primeira. A imagem de fundo **nunca é exibida**. | Corrigido para: `background: url('...') center/cover fixed no-repeat`. |
| C04 | ✅ | `FitUP/Pages/MonteTreino.razor` | 1321 | **Vazamento de memória/timer.** O componente cria um `System.Timers.Timer` no método `StartLoading()` (linha 1129), e o método `Dispose()` existe (linha 1321) mas a classe **não implementa `IDisposable`** — o timer nunca é liberado. | Adicionado `@implements IDisposable` na linha 12. O ciclo de vida Blazor agora chama Dispose corretamente. |
| C05 | ✅ | `FitUP/Components/WelcomeCard.razor` + `FitUP/Pages/Home.razor` | — | **Componente duplicado.** Existe um `WelcomeCard.razor` como componente separado E um card de boas-vindas inline no `Home.razor` (linhas 7–51). Ambos fazem a mesma coisa com chaves localStorage diferentes (`fitup_welcome_v3` vs `fitup_welcome_home`). O componente `WelcomeCard.razor` tem debugs visíveis (linha 19: `<pre>DEBUG: _done=@_done _visible=@_visible</pre>`) e **não é usado em lugar nenhum**. | Remover o componente `WelcomeCard.razor` ou integrá-lo ao `Home.razor`. Remover o `<pre>` de debug. Unificar a chave de localStorage. |
| C06 | ✅ | `FitUP/Pages/MonteTreino.razor` | 1223 | **Parsing frágil de repetições.** `int.TryParse(ex.Reps.Split('–','-')[0]` — para strings como `"45–60s"`, `"10 cada"`, `"2 min"`, o parse retorna `45`, `10`, `2`, mas o valor real de repetições não é capturado corretamente. Exercícios como prancha isométrica e corrida intervalada terão dados incorretos ao salvar. | Substituído pelo método `ParseRepeticoes` que usa regex `\d+` para extrair o primeiro inteiro da string. Corrige "45–60s" → 45, "10 cada" → 10, "2 min" → 2, "1 min forte" → 1. |
| C07 | ✅ | `FitUP/Pages/GeradorDieta.razor` (363) vs `FitUP/Pages/CalculadoraBio.razor` (404-408) | — | **Fórmulas TMB divergentes.** `CalculadoraBio` usa Harris-Benedict com coeficientes precisos (`88.362 + 13.397*P + 4.799*A - 5.677*I`), enquanto `GeradorDieta` usa versão simplificada (`88.36 + 13.4*P + 4.8*A - 5.7*I`). Os resultados serão diferentes entre as duas calculadoras para o mesmo usuário. | Fórmula do GeradorDieta atualizada para os coeficientes precisos de Harris-Benedict (agora idêntica à CalculadoraBio). |
| C08 | ✅ | `FitUP/Layout/MainLayout.razor` | 48 | **Double-toggle no menu do usuário.** O `@onclick` com `ToggleMenuAsync` combinado com `@onclick:stopPropagation` pode causar abertura e fechamento instantâneo no MudBlazor. | Removido `@onclick` manual, usando comportamento padrão do `MudMenu`. |

---

## 🟠 Problemas de Arquitetura e Manutenibilidade

| ID | Status | Arquivo | Linha | Problema | Sugestão de Correção |
|----|--------|---------|-------|----------|----------------------|
| A01 | ✅ | `FitUP/Pages/DicasTreino.razor` | — | **2691 linhas em um único arquivo.** Todo o conteúdo educativo de 12 grupamentos musculares (Peito, Costas, Ombro, Bíceps, Tríceps, Antebraço, Abdômen, Lombar, Quadríceps, Posterior, Panturrilha, Glúteo) está hardcoded no template Razor. | 12 componentes extraídos para `Components/Grupamentos/Dica*.razor`. DicasTreino reduzido de 2691 para ~200 linhas. |
| A02 | ✅ | `FitUP/Pages/MonteTreino.razor` | — | **1443 linhas.** Contém dicionários gigantes de exercícios (150+ exercícios), mapeamentos de foco e 7 templates de workout diretamente no code-behind. | Catálogo extraído para `ExerciseCatalogService` + 3 JSONs: `exercises.json` (70 exercícios), `focus-mappings.json` (15 mapeamentos), `workout-templates.json` (7 templates). `MonteTreino.razor` usa o serviço via DI com `OnInitializedAsync`. |
| A03 | ⬜ | `FitUP/Pages/GanhoMaximo.razor` | — | **1643 linhas.** Contém os dados de 4 dietas completas com alimentos e lógica de exportação de PDF inline no code-behind (método `ExportarDietaPdf` tem 148 linhas com objetos anônimos gigantes). | Extrair dietas para um serviço `DietaDataService`. Componentizar cada seção (Proteínas, Carboidratos, Frutas, Vegetais, Gorduras). |
| A04 | ✅ | `FitUP/Pages/GeradorDieta.razor` | 378-417 | **Banco de alimentos hardcoded.** 30+ alimentos categorizados diretamente no code-behind (método `InicializarBancoAlimentos`). Sem possibilidade de atualização sem recompilar. | 37 alimentos extraídos para `wwwroot/data/alimentos.json`. GeradorDieta carrega via `HttpClient` com fallback hardcoded. |
| A05 | ✅ | `FitUP/Pages/DicasTreino.razor.cs` | 53-62 | **8 técnicas avançadas hardcoded.** Dados de técnicas como Drop Set, Rest-Pause, Bi-Set, Pirâmide, Super Slow, Pré-Exaustão, Cluster Set e Oclusão com nomes, descrições, explicações e passos práticos todos em C#. | 8 técnicas extraídas para `wwwroot/data/tecnicas.json` com estrutura completa (nome, descrição, benefícios, passos práticos). |
| A06 | ✅ | `FitUP/Pages/Termo&Uso.razor` | 2 | **Import desnecessário** e **nome de arquivo problemático.** `@using MudBlazor.Charts` nunca usado. O caractere `&` no nome do arquivo pode causar problemas em alguns sistemas de arquivos e ferramentas. | Remover o using. Renomear arquivo para `Termos.razor`. |
| A07 | ✅ | `FitUP/Pages/Cadastro.razor` | 103 | **Máscara de telefone rígida.** `PatternMask("00000000000")` — 11 dígitos fixos. Não aceita formatação flexível com parênteses e traço, nem valida DDD + número corretamente. | Substituído por `PatternMask("(00) 00000-0000")` com placeholder formatado `Ex: (11) 91234-5678`. |

---

## 🟡 Problemas de Segurança, UX e Boas Práticas

| ID | Status | Arquivo | Linha | Problema | Sugestão de Correção |
|----|--------|---------|-------|----------|----------------------|
| M01 | ⬜ | `FitUP/Services/AuthService.cs` | 199, 343 | **Token JWT em localStorage.** Vulnerável a ataques XSS. Se um script malicioso for injetado, o token pode ser roubado. | Para aplicações Blazor WASM, esta é uma limitação conhecida. Mitigar com Content Security Policy (CSP) restritiva e considerar um BFF (Backend for Frontend) para cookies HttpOnly em produção. |
| M02 | ✅ | `FitUP/Services/ApiResponse.cs` + AuthService, PlanoTreinoService, PlanoAlimentarService, BioimpedanciaService | — | **Tratamento de erro silencioso.** Em caso de falha na API, os métodos retornavam `null`, `false` ou listas vazias sem informar a causa. | Implementada classe `ApiResponse<T>` genérica com `Success`, `ErrorMessage`, `StatusCode`. Factory methods `Ok()`, `Fail()` e `NetworkError()`. Todos os 4 Services e 10 páginas consumidoras atualizados. |
| M03 | ✅ | `FitUP/Pages/EsqueciSenhaDialog.razor` | 25, 95-97 | **Grave vulnerabilidade de segurança.** Alerta "Modo desenvolvimento: o link de redefinição será exibido na tela" e o código copiava o link para o clipboard + exibia em snackbar. O link de reset era completamente exposto. | Removido alerta de dev e código de clipboard. Substituído por mensagem genérica: "Um e-mail com instruções foi enviado". O frontend não expõe mais o link de reset. |
| M04 | ⬜ | `vercel.json` | — | **Deploy apenas do frontend.** A configuração publica somente o `publish/wwwroot` (Blazor WASM). A API em `localhost:5000` precisa ser hospedada separadamente. Atualmente, login, cadastro, salvamento de treinos/dietas — **nada funciona em produção**. | Publicar o backend em um serviço de nuvem (Azure App Service, Render, Fly.io) e configurar CORS. Atualizar `vercel.json` ou configurar um proxy reverso. |
| M05 | ✅ | `FitUP/Pages/Cadastro.razor` | 113-116 | **Validação de CPF insuficiente.** Apenas `[Required]` com máscara visual. Não há validação dos dígitos verificadores do CPF. | Implementado validador de CPF com algoritmo dos dígitos verificadores no frontend. |
| M06 | ⬜ | Várias páginas | — | **Indicador de carregamento ausente.** Páginas como Perfil (carregamento de registros de bioimpedância), CalculadoraBio, GeradorDieta (ao gerar) não mostram feedback visual claro durante chamadas assíncronas. | Adicionar `MudProgressCircular` ou skeletons enquanto as operações estão em andamento. |
| M07 | ⬜ | Todos os Services | — | **Ausência de paginação.** `ListarAsync()` retorna todos os registros. Um usuário com centenas de treinos/dietas sofrerá com performance. | Implementar paginação com parâmetros `page` e `pageSize` tanto no frontend quanto (presumivelmente) no backend. |
| M08 | ⬜ | `FitUP/Pages/Perfil.razor` | 481-493 | **`SepararNomeSobrenome` frágil.** Se o usuário tiver apenas um nome (ex: "João"), o sobrenome fica `""`. O envio para API com `Sobrenome = ""` pode causar erro 400 se o backend exigir sobrenome. | Tratar o caso de nome único (usar string vazia ou repetir o nome). Validar requisitos do backend. |
| M09 | ⬜ | `FitUP/Pages/Perfil.razor` | 447-478 | **Alteração de e-mail sobrescreve nome.** Ao alterar e-mail, o código extrai `Nome` e `Sobrenome` do campo de nome e envia junto com o novo e-mail. Se o usuário só queria trocar o e-mail, o nome também é atualizado desnecessariamente. | Enviar apenas os campos que o usuário realmente quer alterar (usar `null` para campos não modificados, pois o backend usa `COALESCE`). |
| M10 | ✅ | `FitUP/App.razor` | — | **Arquivo `NotFound.razor` órfão.** O `App.razor` tem um template inline de 404. O arquivo `NotFound.razor` existe no diretório Pages mas parece ser um leftover não utilizado. | Verificar se `NotFound.razor` está obsoleto. Remover se não for usado, ou integrá-lo ao `App.razor`. |

---

## 🟢 Melhorias e Observações

| ID | Status | Arquivo | Linha | Observação | Sugestão |
|----|--------|---------|-------|------------|----------|
| B01 | ✅ | `FitUP/Pages/GanhoMaximo.razor` e outros | — | **Typos em classes CSS:** `trasition-fast-out-slow-in` (deveria ser `transition`) aparece em múltiplos arquivos: `Home.razor`, `GanhoMaximo.razor`, `MonteTreino.razor`. | Substituir `trasition` por `transition` em todos os arquivos. |
| B02 | ✅ | `FitUP/Pages/GanhoMaximo.razor` | 1458-1643 | **Code-behind com 186 linhas.** Contém 5 bools, 5 métodos toggle idênticos e 4 objetos gigantes de dieta inline (cada um com 30-60 linhas). | Componente `ToggleSection.razor` criado em `Components/GanhoMaximo/`. Integração no `GanhoMaximo.razor` pendente (A03). |
| B03 | ✅ | `FitUP/Pages/MonteTreino.razor` | 1114-1122 | **`MatchesBlockedKey` tem complexidade O(n²).** Para cada exercício, itera todo o catálogo de exercícios (`_exerciseCatalog`). Com 150+ exercícios, isso é ineficiente. | Dicionário reverso `ExerciseNameToKey` implementado no `ExerciseCatalogService`. `MatchesBlockedKey` agora é O(1) via `TryGetKeyByName`. |
| B04 | ✅ | `FitUP/wwwroot/index.html` | 16 | **jsPDF via CDN externo.** `cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js` — se o CDN estiver offline, a exportação de PDF quebra completamente. Sem fallback. | jsPDF baixado e servido localmente em `wwwroot/js/jspdf.umd.min.js`. |
| B05 | ✅ | `FitUP/Pages/Login.razor`, `Cadastro.razor` | — | **Botões com texto fixo e loading.** No Cadastro, o texto "CRIAR CONTA" desaparece quando `_isLoading` é true (só mostra spinner sem texto). No Login mostra spinner + "ENTRAR" (mais consistente). | Padronizar: manter texto + spinner durante loading em todas as páginas. |

---

## 🎯 Próximos Passos (Ordem de Prioridade)

1. **Recuperar/recriar o backend** (`src/FitUP.WebApi/`) — dependência bloqueante para todas as funcionalidades
2. **Corrigir o CSS do background** no `MainLayout.razor` — a experiência visual está quebrada
3. **Implementar `IDisposable`** corretamente no `MonteTreino.razor` — corrigir vazamento de memória
4. **Remover o componente WelcomeCard duplicado** — limpar código morto e debug
5. ~~**Resolver a exposição do link de reset** no `EsqueciSenhaDialog.razor`~~ ✅ Fase 1 — M03
6. **Externalizar dados hardcoded**: catálogo de exercícios, banco de alimentos, conteúdo de dicas
7. **Uniformizar fórmulas TMB** entre as duas calculadoras
8. **Configurar URL da API** via `appsettings.json` com suporte a ambientes
9. ~~**Adicionar tratamento de erro adequado** nos Services (status code, logging)~~ ✅ Fase 1 — M02
10. **Componentizar páginas gigantes** (DicasTreino, MonteTreino, GanhoMaximo)

---

> 📝 **Nota:** Marque os itens como concluídos alterando `⬜` para `✅` quando forem resolvidos. Este documento deve ser atualizado a cada ciclo de correção.