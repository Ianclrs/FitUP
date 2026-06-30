# SPEC-010: Validação server-side no registro de usuário

**Prioridade:** Alta
**Dependências:** Nenhuma
**Status:** Não iniciado
**Progresso:** ░░░░░░░░░░░░░░░░░░░░ 0% (0/5)

> **Alinhado com:** `visao.md` §2 (Estado atual — bugs back-end), `Roadmap.md` Fase 2
> **Sub-specs:** se houver impedimento, criar SPEC-011, SPEC-012...

---

## Objetivo

Adicionar validação no back-end para o endpoint `POST /api/auth/registrar`. Atualmente a API aceita qualquer payload sem verificar a qualidade dos dados — a validação só existe no front-end, e o Swagger/Postman pode burlá-la.

## Requisitos

### R1 — Senha mínima
- Senha deve ter no mínimo **6 caracteres**.
- Se menor, retornar `400 Bad Request` com `{ "mensagem": "A senha deve ter no mínimo 6 caracteres." }`.

### R2 — Email com formato válido
- Validar com `System.Net.Mail.MailAddress` ou regex simples (contém `@` e `.` após o `@`).
- Se inválido, retornar `400 Bad Request` com `{ "mensagem": "Formato de email inválido." }`.

### R3 — Data de nascimento não futura
- Se `DataNascimento` for informada e for maior que `DateTime.UtcNow`, retornar `400 Bad Request` com `{ "mensagem": "Data de nascimento não pode ser futura." }`.

### R4 — Nome e Sobrenome não vazios
- `Nome` e `Sobrenome` não podem ser string vazia ou só espaços.
- Se vazios, retornar `400 Bad Request` com `{ "mensagem": "Nome e sobrenome são obrigatórios." }`.

## Onde mexer

- **Arquivo:** `src/FitUP.WebApi/Service/AuthService.cs`
- **Método:** `RegistrarAsync`
- **Antes** do `await connection.OpenAsync()`, inserir as validações e retornar `null` se inválidas (o Controller já trata `null` como `409 Conflict` — será necessário mudar para um padrão que diferencie conflito de validação).

## Atenção

O `AuthController.Registrar` atualmente retorna `Conflict` quando `RegistrarAsync` retorna `null`. Como agora teremos dois motivos de falha (conflito de email × validação), ideal refatorar para retornar um objeto de erro ou usar exceções. **Sugestão:** `RegistrarAsync` passa a lançar `ArgumentException` para validação, e o Controller captura e retorna `400`.

## Critério de aceitação

- [ ] Swagger: enviar `{ "senha": "123" }` retorna 400 com mensagem clara.
- [ ] Swagger: enviar `{ "email": "x" }` retorna 400.
- [ ] Swagger: enviar `{ "dataNascimento": "2099-01-01" }` retorna 400.
- [ ] Swagger: enviar payload válido continua retornando 201.
- [ ] Swagger: email duplicado continua retornando 409.
