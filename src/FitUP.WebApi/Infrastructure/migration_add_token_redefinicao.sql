-- Migração: Adiciona colunas TokenRedefinicao e TokenRedefinicaoExpiraEm na tabela Usuario
-- Data: 2026-07-13
-- Descrição: Resolve erro "Nome de coluna 'TokenRedefinicao' inválido" no endpoint EsqueciSenha

-- Verifica se a coluna já existe antes de adicionar (evita erro se já foi executada)
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE Name = N'TokenRedefinicao' AND Object_ID = Object_ID(N'Usuario')
)
BEGIN
    ALTER TABLE Usuario
    ADD TokenRedefinicao NVARCHAR(500) NULL;
    
    PRINT 'Coluna TokenRedefinicao adicionada com sucesso.';
END
ELSE
BEGIN
    PRINT 'Coluna TokenRedefinicao já existe. Nenhuma alteração necessária.';
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE Name = N'TokenRedefinicaoExpiraEm' AND Object_ID = Object_ID(N'Usuario')
)
BEGIN
    ALTER TABLE Usuario
    ADD TokenRedefinicaoExpiraEm DATETIME2 NULL;
    
    PRINT 'Coluna TokenRedefinicaoExpiraEm adicionada com sucesso.';
END
ELSE
BEGIN
    PRINT 'Coluna TokenRedefinicaoExpiraEm já existe. Nenhuma alteração necessária.';
END
GO

-- Verificação final
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Usuario'
  AND COLUMN_NAME IN ('TokenRedefinicao', 'TokenRedefinicaoExpiraEm');
GO