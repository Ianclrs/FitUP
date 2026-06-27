# ADR-0003: Soft delete para Usuario, hard delete CASCADE para planos

**Data:** 2026-06-27
**Status:** Aceito

---

### Contexto

O sistema tem duas hierarquias de dados principais: usuários com seus planos (treino + alimentar) e registros de bioimpedância. Era necessário decidir o que acontece com os dados quando um usuário "deleta" sua conta, e quando um plano ou registro é removido.

Duas abordagens conflitantes: preservar tudo (soft delete) ou remover tudo (hard delete). A decisão impacta: recuperação de dados, integridade referencial, storage e simplicidade do código.

### Opções consideradas

- **Opção A — Soft delete universal:** Toda tabela ganha coluna `Ativo`/`DeletadoEm`. Nada é removido fisicamente. Queries sempre incluem `WHERE Ativo = 1`.
- **Opção B — Hard delete universal:** `DELETE FROM` remove fisicamente. FKs com `ON DELETE CASCADE` limpam a árvore inteira.
- **Opção C — Híbrido (soft delete no Usuario, hard delete CASCADE nos planos):** Usuario tem `Ativo` (BIT). Desativar = `Ativo = 0`. Planos e registros são removidos fisicamente com `DELETE` + `ON DELETE CASCADE`.

### Decisão

**Escolhida: Opção C — Híbrido.**

Motivos:

1. **Usuario merece preservação.** Dados cadastrais (Nome, Email, CPF) têm implicações legais e de auditoria. Um soft delete permite reativação futura e mantém histórico de quem usou o sistema.
2. **Planos são descartáveis.** Um PlanoTreino com seus Dias e Exercícios é conteúdo gerado pelo usuário. Se ele quer remover, é uma ação intencional e consciente. Não há valor em manter exercícios órfãos.
3. **CASCADE simplifica o código.** `DELETE FROM PlanoTreino WHERE Id = @Id` → SQL Server remove automaticamente DiaTreino → Exercicio. Sem código adicional para limpar filhos. Menos chão para bug de dados órfãos.
4. **Bioimpedância segue a mesma lógica.** Registros são medições pontuais. Remover um registro não quebra nada — é uma ação esperada do usuário.

### Consequências

- ✅ **Usuário reativável.** `Ativo = 0` pode ser revertido. Dados de planos antigos continuam acessíveis se o usuário for reativado.
- ✅ **FKs limpam a casa.** Sem dados órfãos. Sem `WHERE DeletadoEm IS NULL` em toda query. Sem JOINs condicionais.
- ✅ **Código simples.** `DELETE FROM` direto. Sem abstração de "deletar = atualizar flag".
- ✅ **Auth verifica Ativo.** `LoginAsync` já verifica `Ativo = 1` antes de gerar token. Usuário desativado não faz login.

- ⚠️ **Sem "lixeira" para planos.** Se o usuário deletar um plano por engano, não há como recuperar. Não existe UI ou endpoint de restore. Mitigável com diálogo de confirmação no front-end.
- ⚠️ **CASCADE é agressivo.** `DELETE FROM PlanoTreino` remove dias e exercícios em cascata. Se um dia for compartilhado entre planos (não é o caso hoje), seria um problema. O schema atual não tem compartilhamento, então é seguro.
- ⚠️ **Dados de usuário acumulam.** Usuários desativados permanecem no banco para sempre. Em produção com milhões de contas, uma política de expurgo seria necessária (GDPR, LGPD).

---

*ADR-0003. Consulte todos os ADRs existentes antes de propor mudanças arquiteturais.*
