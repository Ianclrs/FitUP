# ADR-0002: UNIQUEIDENTIFIER (Guid) como chave primária

**Data:** 2026-06-27
**Status:** Aceito

---

### Contexto

Ao modelar as 8 tabelas do banco, era necessário decidir o tipo das chaves primárias. As opções padrão no SQL Server são `INT IDENTITY` (autoincremento numérico) ou `UNIQUEIDENTIFIER` (Guid). A decisão impactaria: exposição de IDs em URLs da API, facilidade de merge de dados entre ambientes, performance de índices e segurança por obscuridade.

### Opções consideradas

- **Opção A — INT IDENTITY (1, 2, 3...):** Numérico sequencial, 4 bytes, índice clusterizado eficiente. IDs previsíveis (`/api/planos-treino/42`).
- **Opção B — UNIQUEIDENTIFIER com NEWID():** Guid aleatório, 16 bytes. Índice fragmentado por falta de ordenação.
- **Opção C — UNIQUEIDENTIFIER com NEWSEQUENTIALID():** Guid sequencial, 16 bytes. Melhor que NEWID() para índices clusterizados porque gera valores ordenados.

### Decisão

**Escolhida: Opção C — UNIQUEIDENTIFIER com DEFAULT NEWSEQUENTIALID().**

Motivos:

1. **IDs não previsíveis.** Em um sistema multi-tenant (vários usuários), um ID sequencial permite enumeração: `GET /api/planos-treino/41`, `GET /api/planos-treino/42`. Um Guid evita isso sem precisar de lógica extra de autorização por recurso.
2. **Independência de ambiente.** Se dados forem migrados entre dev/staging/prod, Guids não colidem. Com INT IDENTITY, os mesmos IDs existiriam em ambientes diferentes, gerando conflitos.
3. **NEWSEQUENTIALID() resolve a fragmentação.** Diferente de `NEWID()` (totalmente aleatório), o `NEWSEQUENTIALID()` gera Guids em ordem crescente na máquina local. O índice clusterizado da PK não sofre page splits excessivos.
4. **Consistência com o ecossistema .NET.** `Guid` é tipo nativo em C#. `Guid.NewGuid()` no código ↔ `NEWSEQUENTIALID()` no banco.

### Consequências

- ✅ **IDs não enumeráveis.** Segurança por obscuridade — um usuário não descobre quantos registros existem nem acessa recursos de outros por tentativa.
- ✅ **Merge seguro entre ambientes.** Sem risco de colisão de PK ao migrar dados.
- ✅ **Índice clusterizado saudável.** `NEWSEQUENTIALID()` mitiga o problema clássico de fragmentação de Guid.
- ✅ **Mapeamento direto C# ↔ SQL.** `Guid` no modelo, `UNIQUEIDENTIFIER` na tabela. Sem conversão.
- ✅ **Controllers com constraint de rota.** `[HttpGet("{id:guid}")]` garante que só Guids válidos chegam ao Controller.

- ⚠️ **16 bytes por PK.** O dobro de `BIGINT` (8 bytes) e 4x `INT` (4 bytes). Com FKs em cascata, o overhead se multiplica em tabelas filhas (DiaTreino, Exercicio, Refeicao, Alimento).
- ⚠️ **URLs longas.** `/api/planos-treino/3fa85f64-5717-4562-b3fc-2c963f66afa6` vs `/api/planos-treino/42`. Aceitável para API, mas feio em logs.
- ⚠️ **Debug manual mais difícil.** Fazer `SELECT * WHERE Id = '3fa85f64-...'` no SSMS é mais chato do que `WHERE Id = 42`.

---

*ADR-0002. Consulte todos os ADRs existentes antes de propor mudanças arquiteturais.*
