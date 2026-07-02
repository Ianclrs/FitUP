# FitUP — Lições Aprendidas

> **⚠️ LEIA ESTE ARQUIVO INTEIRO ANTES DE QUALQUER COISA.**  
> Ele é a memória entre sessões. Todo erro grave, descoberta de projeto ou armadilha técnica fica registrado aqui.  
> **Nunca repita um erro já listado.**

---

### Chave de configuração errada no TokenService — 2026-06-27

**Contexto:** O `TokenService.cs` gera tokens JWT com tempo de expiração configurável via `appsettings.json`.

**Problema:** O código lê a chave `jwtSettings["ExpireMinutes"]`, mas o `appsettings.json` define `"ExpiresInMinutes"`. Nomes diferentes. O `ExpireMinutes` sempre retorna `null`, então o fallback `?? "120"` é usado. Alterar `ExpiresInMinutes` no config **não muda nada** — o token sempre expira em 120 minutos.

**Solução:** Trocar a linha para:
```csharp
double.Parse(jwtSettings["ExpiresInMinutes"] ?? "120")
```

**Nunca mais:** Sempre verificar se a chave lida no código casa exatamente com a escrita no `appsettings.json` — letra por letra. Preferir `nameof()` ou constantes em vez de string mágica.

---

### Reader.Close() manual antes de UPDATE no AuthService — 2026-06-27

**Contexto:** Nos métodos `LoginAsync` e `RefreshTokenAsync` do `AuthService.cs`, após ler o usuário com `SqlDataReader`, o código chama `reader.Close()` manualmente e depois executa um `UPDATE` na mesma conexão.

**Problema:** Se o `UPDATE` falhar (timeout, deadlock, conexão dropada), o JWT e o refresh token **já foram gerados e retornados ao cliente**, mas **não foram persistidos no banco**. O refresh token enviado ao front-end é inválido. Na próxima renovação, o usuário recebe 401 sem motivo aparente.

**Solução:** Inverter a ordem: primeiro fazer o `UPDATE` (persistir refresh token), só depois gerar e retornar o JWT. Ou usar transação (`SqlTransaction`) para atomicidade.

**Nunca mais:** Operações que geram credenciais (token, refresh token) devem persistir o estado no banco **antes** de retornar ao cliente. Se a persistência falhar, o cliente nunca recebe uma credencial inválida.

---

### AddWithValue infere tipo e pode quebrar plano de execução — 2026-06-27

**Contexto:** Todos os Services usam `command.Parameters.AddWithValue("@Nome", valor)` para passar parâmetros SQL.

**Problema:** `AddWithValue` infere o `SqlDbType` a partir do tipo .NET. Para `string`, infere `NVARCHAR(4000)`. Para `int`, `INT`. Isso funciona, mas o SQL Server compila um plano de execução baseado no tipo inferido. Se o tipo inferido não bater com o tipo real da coluna (ex: `NVARCHAR(100)` na tabela vs `NVARCHAR(4000)` inferido), o otimizador pode escolher um plano subótimo ou não usar índices corretamente.

**Solução:** Substituir por:
```csharp
command.Parameters.Add("@Nome", SqlDbType.NVarChar, 100).Value = valor;
```

**Nunca mais:** Em queries com índice ou alta cardinalidade, usar `Add` com tipo e tamanho explícitos. `AddWithValue` é aceitável para queries triviais, mas o hábito correto é sempre explicitar.

---

### N+1 queries em listagens de planos — 2026-06-27

**Contexto:** `PlanoTreinoService.ListarPorUsuarioAsync` e `PlanoAlimentarService.ListarPorUsuarioAsync` buscam primeiro os IDs dos planos, depois chamam `ObterPorIdAsync` para cada ID individualmente.

**Problema:** Cada chamada a `ObterPorIdAsync` abre uma nova `SqlConnection`, faz JOINs, e fecha. Para 10 planos = 11 conexões e 21 queries. O `RegistroBioimpedanciaService` não tem esse problema — ele faz uma query única.

**Solução:** JOIN único que retorne todos os planos com dias e exercícios de uma vez, agrupando no código via `Dictionary<Guid, PlanoTreinoDto>`.

**Nunca mais:** Toda listagem que precise de dados aninhados deve usar JOIN único. Nunca fazer loop de `SELECT` individual — o banco é pago para fazer JOIN.

---
