-- ============================================================
-- FitUP - Script de Criação do Banco de Dados
-- SQL Server Express 2025
-- ============================================================

-- Criar banco de dados
CREATE DATABASE FitUP;
GO

USE FitUP;
GO

-- ============================================================
-- TABELA: Usuario
-- ============================================================
CREATE TABLE Usuario (
    Id                      UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    Nome                    NVARCHAR(100)       NOT NULL,
    Sobrenome               NVARCHAR(100)       NOT NULL,
    Email                   NVARCHAR(200)       NOT NULL,
    SenhaHash               NVARCHAR(MAX)       NOT NULL,
    Telefone                NVARCHAR(20)        NULL,
    CPF                     NVARCHAR(14)        NULL,
    DataNascimento          DATE                NULL,
    CriadoEm                DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    UltimoLoginEm           DATETIME2           NULL,
    Ativo                   BIT                 NOT NULL DEFAULT 1,
    RefreshToken            NVARCHAR(500)       NULL,
    RefreshTokenExpiraEm    DATETIME2           NULL,

    CONSTRAINT UQ_Usuario_Email UNIQUE (Email)
);
GO

-- ============================================================
-- TABELA: PlanoTreino
-- ============================================================
CREATE TABLE PlanoTreino (
    Id                  UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UsuarioId           UNIQUEIDENTIFIER    NOT NULL,
    Nome                NVARCHAR(150)       NOT NULL,
    Descricao           NVARCHAR(500)       NULL,
    Divisao             INT                 NOT NULL DEFAULT 0,
    Nivel               INT                 NOT NULL DEFAULT 0,
    FrequenciaSemanal   INT                 NOT NULL DEFAULT 0,
    CriadoEm            DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    AtualizadoEm        DATETIME2           NULL,

    CONSTRAINT FK_PlanoTreino_Usuario FOREIGN KEY (UsuarioId)
        REFERENCES Usuario(Id)
        ON DELETE CASCADE
);
GO

-- ============================================================
-- TABELA: DiaTreino
-- ============================================================
CREATE TABLE DiaTreino (
    Id              UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    PlanoTreinoId   UNIQUEIDENTIFIER    NOT NULL,
    Nome            NVARCHAR(100)       NOT NULL,
    Ordem           INT                 NOT NULL DEFAULT 0,

    CONSTRAINT FK_DiaTreino_PlanoTreino FOREIGN KEY (PlanoTreinoId)
        REFERENCES PlanoTreino(Id)
        ON DELETE CASCADE
);
GO

-- ============================================================
-- TABELA: Exercicio
-- ============================================================
CREATE TABLE Exercicio (
    Id              UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    DiaTreinoId     UNIQUEIDENTIFIER    NOT NULL,
    Nome            NVARCHAR(150)       NOT NULL,
    Descricao       NVARCHAR(500)       NULL,
    GrupoMuscular   INT                 NOT NULL DEFAULT 0,
    Series          INT                 NOT NULL DEFAULT 0,
    Repeticoes      INT                 NOT NULL DEFAULT 0,
    Carga           FLOAT               NULL,
    Observacoes     NVARCHAR(500)       NULL,
    Ordem           INT                 NOT NULL DEFAULT 0,

    CONSTRAINT FK_Exercicio_DiaTreino FOREIGN KEY (DiaTreinoId)
        REFERENCES DiaTreino(Id)
        ON DELETE CASCADE
);
GO

-- ============================================================
-- TABELA: PlanoAlimentar
-- ============================================================
CREATE TABLE PlanoAlimentar (
    Id              UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UsuarioId       UNIQUEIDENTIFIER    NOT NULL,
    Nome            NVARCHAR(150)       NOT NULL,
    Objetivo        INT                 NOT NULL DEFAULT 0,
    Descricao       NVARCHAR(500)       NULL,
    CriadoEm        DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    AtualizadoEm    DATETIME2           NULL,

    CONSTRAINT FK_PlanoAlimentar_Usuario FOREIGN KEY (UsuarioId)
        REFERENCES Usuario(Id)
        ON DELETE CASCADE
);
GO

-- ============================================================
-- TABELA: Refeicao
-- ============================================================
CREATE TABLE Refeicao (
    Id                  UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    PlanoAlimentarId    UNIQUEIDENTIFIER    NOT NULL,
    Nome                NVARCHAR(100)       NOT NULL,
    HorarioSugerido     TIME                NULL,
    Ordem               INT                 NOT NULL DEFAULT 0,
    TotalProteina       FLOAT               NOT NULL DEFAULT 0,
    TotalCarboidrato    FLOAT               NOT NULL DEFAULT 0,
    TotalGordura        FLOAT               NOT NULL DEFAULT 0,
    TotalFibra          FLOAT               NOT NULL DEFAULT 0,
    TotalCalorias       FLOAT               NOT NULL DEFAULT 0,

    CONSTRAINT FK_Refeicao_PlanoAlimentar FOREIGN KEY (PlanoAlimentarId)
        REFERENCES PlanoAlimentar(Id)
        ON DELETE CASCADE
);
GO

-- ============================================================
-- TABELA: Alimento
-- ============================================================
CREATE TABLE Alimento (
    Id              UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    RefeicaoId      UNIQUEIDENTIFIER    NOT NULL,
    Nome            NVARCHAR(150)       NOT NULL,
    Quantidade      FLOAT               NOT NULL DEFAULT 0,
    UnidadeMedida   NVARCHAR(20)        NOT NULL DEFAULT 'g',
    Proteina        FLOAT               NOT NULL DEFAULT 0,
    Carboidrato     FLOAT               NOT NULL DEFAULT 0,
    Gordura         FLOAT               NOT NULL DEFAULT 0,
    Fibra           FLOAT               NOT NULL DEFAULT 0,
    Calorias        FLOAT               NOT NULL DEFAULT 0,
    Observacoes     NVARCHAR(300)       NULL,

    CONSTRAINT FK_Alimento_Refeicao FOREIGN KEY (RefeicaoId)
        REFERENCES Refeicao(Id)
        ON DELETE CASCADE
);
GO

-- ============================================================
-- TABELA: RegistroBioimpedancia
-- ============================================================
CREATE TABLE RegistroBioimpedancia (
    Id                          UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UsuarioId                   UNIQUEIDENTIFIER    NOT NULL,
    DataRegistro                DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    Peso                        FLOAT               NOT NULL DEFAULT 0,
    Altura                      FLOAT               NOT NULL DEFAULT 0,
    MassaMagra                  FLOAT               NULL,
    MassaGorda                  FLOAT               NULL,
    PercentualGordura           FLOAT               NULL,
    MassaMuscular               FLOAT               NULL,
    AguaCorporal                FLOAT               NULL,
    TaxaMetabolicaBasal         FLOAT               NULL,
    IdadeMetabolica             FLOAT               NULL,
    CircunferenciaCintura       FLOAT               NULL,
    CircunferenciaQuadril       FLOAT               NULL,
    RelacaoCinturaQuadril       FLOAT               NULL,
    Observacoes                 NVARCHAR(500)       NULL,

    CONSTRAINT FK_RegistroBioimpedancia_Usuario FOREIGN KEY (UsuarioId)
        REFERENCES Usuario(Id)
        ON DELETE CASCADE
);
GO

-- ============================================================
-- ÍNDICES
-- ============================================================
CREATE INDEX IX_PlanoTreino_UsuarioId ON PlanoTreino(UsuarioId);
CREATE INDEX IX_DiaTreino_PlanoTreinoId ON DiaTreino(PlanoTreinoId);
CREATE INDEX IX_Exercicio_DiaTreinoId ON Exercicio(DiaTreinoId);
CREATE INDEX IX_PlanoAlimentar_UsuarioId ON PlanoAlimentar(UsuarioId);
CREATE INDEX IX_Refeicao_PlanoAlimentarId ON Refeicao(PlanoAlimentarId);
CREATE INDEX IX_Alimento_RefeicaoId ON Alimento(RefeicaoId);
CREATE INDEX IX_RegistroBioimpedancia_UsuarioId ON RegistroBioimpedancia(UsuarioId);
GO
