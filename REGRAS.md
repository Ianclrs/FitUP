# REGRAS.md — FitUP

> **Leitura obrigatória antes de qualquer ação.**  
> Estas regras regem todo o desenvolvimento do projeto. A IA deve segui-las sem exceção.

---

## 1. Idioma

- Todo o projeto deve ser desenvolvido em **Português (PT-BR)**.
- Documentações, comentários, mensagens de erro, validações e textos exibidos ao usuário: **Português**.
- Nomes técnicos de classes, métodos, bibliotecas e tecnologias podem permanecer em **inglês** quando fizer sentido.

---

## 2. Leitura Obrigatória

Antes de **QUALQUER** ação ou implementação, a IA deve:

1. Reler todos os arquivos da pasta `/docs`:
   - `visao.md`
   - `Arquitetura.md`
   - `Roadmap.md`
   - `ADR-*.md`
   - `Licoes-aprendidas.md`
2. Reler este `REGRAS.md`.
3. Ler os arquivos de código relacionados à funcionalidade que será alterada.
4. Entender a arquitetura atual do projeto.
5. Verificar se já existe implementação semelhante.

**Esta revisão NÃO é opcional.** Nenhuma alteração, criação de arquivo ou decisão técnica deve ser realizada sem esta análise prévia completa.

---

## 3. Planejamento Obrigatório

Antes de escrever qualquer código:

1. Explicar o que foi entendido da tarefa.
2. Apresentar um plano de implementação.
3. Informar quais arquivos serão modificados.
4. Informar quais arquivos serão criados (caso necessário).
5. **Aguardar aprovação do desenvolvedor.**

---

## 4. Criação de Arquivos e Diretórios

**É proibido:**
- Criar arquivos sem necessidade.
- Criar diretórios sem necessidade.
- Duplicar funcionalidades já existentes.

Caso seja necessário criar arquivos ou diretórios:
1. Explicar o motivo.
2. Informar o benefício.
3. Solicitar aprovação antes da criação.

---

## 5. Alteração de Arquivos

Antes de alterar qualquer arquivo:
1. Explicar o motivo da alteração.
2. Informar o impacto da mudança.
3. Identificar possíveis riscos.

---

## 6. Git e Versionamento

**É proibido:**
- Criar commits automaticamente.
- Executar push automaticamente.
- Executar merge automaticamente.
- Criar branches automaticamente.

Toda ação Git deve ser aprovada previamente.

### Commits
- Nenhum commit sem autorização explícita.
- Sempre sugerir uma mensagem de commit antes de executá-lo.
- Utilizar **Conventional Commits**.

Exemplos:
```
feat(auth): implementar autenticação JWT
fix(cliente): corrigir validação de CPF
refactor(agendamento): simplificar regra de conflito
```

### Branches
- Utilizar a branch do desenvolvedor: `devpedro`.
- **Nunca** enviar código diretamente para `main`.
- **Nunca** criar branches sem autorização.
- Operações locais de git (`git status`, `git diff`, etc.) são permitidas para consulta.

---

## 7. Qualidade de Código

Todo código deve:
- Seguir **SOLID** quando aplicável.
- Seguir **Clean Code**.
- Evitar duplicação.
- Ser legível e de fácil manutenção.
- Utilizar nomes claros e consistentes.

### KISS em primeiro lugar
- Soluções simples e diretas sempre têm prioridade.
- **DRY** sempre que possível, mas **KISS > DRY**.
- Se para não repetir código for preciso criar algo muito complexo, prefira a repetição.
- Exemplo: dois `switch` simples e repetidos são melhores do que uma abstração genérica difícil de entender.

---

## 8. Segurança

- Nunca armazenar senhas em texto puro.
- Utilizar **BCrypt** para hash de senhas.
- Validar todas as entradas.
- Tratar exceções adequadamente.
- Não expor informações sensíveis.

---

## 9. Banco de Dados

- Não alterar estruturas existentes sem justificativa.
- Não remover tabelas, colunas ou constraints sem aprovação.
- Explicar impactos de qualquer alteração estrutural.

---

## 10. Revisão Obrigatória

Após concluir qualquer implementação:
1. Revisar o código gerado.
2. Identificar possíveis bugs.
3. Identificar riscos de segurança.
4. Identificar melhorias.
5. Apresentar um resumo técnico da implementação.

---

## 11. Regra Principal

A IA deve atuar como **assistente de desenvolvimento**.

A IA **nunca** deve assumir que pode:
- Criar arquivos.
- Excluir arquivos.
- Renomear arquivos.
- Executar comandos Git (commits, pushes, merges).
- Alterar arquitetura.
- Refatorar grandes partes do sistema.

**Sem aprovação explícita do desenvolvedor.**

---

## 12. Numeração de Specs

- As specs principais são numeradas de **10 em 10**: `010`, `020`, `030`, etc.
- Correções de bugs e hotfixes são associados à spec original acrescentando 1:
  - Spec `010` → bug fix: `011`
  - Spec `020` → hotfix: `021`, outro hotfix: `022`
- O número base (dezena) nunca muda, apenas incrementa a unidade.

---

## 13. Regras de Implementação

1. **NÃO** implementar código sem um documento de spec aprovado.
2. Cada spec tem de ser aprovada por **Pedro** antes de qualquer implementação começar.
3. Se Pedro pedir diretamente, pode seguir — a aprovação do dono do produto é suficiente para iniciar.
4. Na dúvida, **NÃO** assuma. Pergunte ao Pedro.
5. Qualquer ambiguidade, decisão de design, ou caminho incerto deve ser esclarecido com Pedro antes de agir.

---

*Regras estabelecidas em Junho/2026. Violação de qualquer regra invalida a implementação.*
