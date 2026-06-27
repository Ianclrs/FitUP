# ADR-0001: ADO.NET puro sem Entity Framework Core

**Data:** 2026-06-27
**Status:** Aceito

---

### Contexto

O projeto FitUP precisava de uma camada de persistência para 8 tabelas relacionais no SQL Server Express 2025. A escolha do mecanismo de acesso a dados impactaria diretamente: complexidade do código, performance de queries, curva de aprendizado do time, facilidade de debug e controle sobre o SQL gerado.

A stack já estava definida como .NET 10 + SQL Server, restando decidir como o back-end se comunicaria com o banco.

### Opções consideradas

- **Opção A — Entity Framework Core 10:** ORM completo com DbContext, LINQ to SQL, migrations, change tracking, lazy/eager loading.
- **Opção B — Dapper:** Micro-ORM. SQL manual com mapeamento automático de resultados para objetos.
- **Opção C — ADO.NET puro (Microsoft.Data.SqlClient):** SQL manual com mapeamento manual via SqlDataReader.

### Decisão

**Escolhida: Opção C — ADO.NET puro com Microsoft.Data.SqlClient 6.*.**

Motivos:

1. **Controle total sobre o SQL.** Toda query é visível e auditável no código. Sem SQL gerado automaticamente, sem surpresas de performance por queries mal otimizadas pelo ORM.
2. **Projeto acadêmico.** O time está aprendendo. ADO.NET força o entendimento real de como a comunicação com o banco funciona — conexões, comandos, parâmetros, readers — sem a abstração de um ORM.
3. **Simplicidade.** Sem DbContext para configurar, sem migrations para gerenciar, sem convenções de naming para decorar. A fonte única de verdade do schema é `script.sql`.
4. **Nenhuma dependência adicional.** `Microsoft.Data.SqlClient` já é a biblioteca nativa de acesso a dados no ecossistema .NET.
5. **Sem camada extra de abstração.** O código SQL está visível e próximo da lógica de negócio nos Services. Para um time pequeno e um domínio com 8 entidades, uma camada Repository ou um ORM adicionariam complexidade desnecessária.

### Consequências

- ✅ **SQL explícita e auditável.** Toda query está no código, visível em code review.
- ✅ **Zero magic.** Sem lazy loading surpresa, sem N+1 queries escondidas atrás de LINQ, sem tracking de entidades.
- ✅ **Dependência única.** Apenas `Microsoft.Data.SqlClient` no `.csproj`. Sem pacotes de ORM, sem providers, sem ferramentas de migration.
- ✅ **Schema versionado como script.** `Infrastructure/script.sql` é a verdade absoluta. `git diff` mostra exatamente o que mudou.
- ✅ **Curva de aprendizado.** Time entende o ciclo completo: abrir conexão → executar comando → ler resultados → fechar.

- ⚠️ **Mapeamento manual verboso.** Cada coluna exige `reader.GetGuid(reader.GetOrdinal("Nome"))`. Para 8+ colunas por tabela, é código repetitivo. Um micro-ORM como Dapper eliminaria isso com `connection.Query<T>()` sem perder controle do SQL.
- ⚠️ **Sem migrations.** Alterações de schema exigem editar `script.sql` e aplicar manualmente no banco. Em produção, isso exigiria scripts de delta ou ferramentas externas.
- ⚠️ **Risco de AddWithValue.** O projeto atual usa `command.Parameters.AddWithValue("@Nome", valor)`, que infere o tipo. Para queries complexas com índices, isso pode gerar planos de execução subótimos. Recomenda-se migrar para `Add("@Nome", SqlDbType.NVarChar, 200).Value = valor`.
- ⚠️ **Sem conexão compartilhada.** Cada método cria `new SqlConnection(_connectionString)`. O ADO.NET tem pool interno por connection string, então isso não é um problema de performance, mas impede transações entre múltiplas operações no mesmo Service.
- ⚠️ **N+1 queries em listagens.** `ListarPorUsuarioAsync` busca IDs primeiro, depois chama `ObterPorIdAsync` para cada item. Com ORM, um `.Include()` resolveria. Aqui exige JOIN manual que ainda não foi implementado.

---

*ADR-0001. Próximo ADR deve usar numeração sequencial (ADR-0002, ADR-0003...). Consulte todos os ADRs existentes antes de propor mudanças arquiteturais.*
