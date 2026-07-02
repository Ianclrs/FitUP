using System.Data;
using FitUP.WebApi.DTOs;
using Microsoft.Data.SqlClient;

namespace FitUP.WebApi.Service;

public class PlanoTreinoService : IPlanoTreinoService
{
    private readonly string _connectionString;

    public PlanoTreinoService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<PlanoTreinoDto?> ObterPorIdAsync(Guid id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string planoSql = @"
            SELECT Id, Nome, Descricao, Divisao, Nivel, FrequenciaSemanal,
                   UsuarioId, CriadoEm, AtualizadoEm
            FROM PlanoTreino WHERE Id = @Id";

        await using var planoCommand = new SqlCommand(planoSql, connection);
        planoCommand.Parameters.AddWithValue("@Id", id);

        await using var planoReader = await planoCommand.ExecuteReaderAsync();
        if (!await planoReader.ReadAsync())
            return null;

        var plano = new PlanoTreinoDto
        {
            Id = planoReader.GetGuid(planoReader.GetOrdinal("Id")),
            Nome = planoReader.GetString(planoReader.GetOrdinal("Nome")),
            Descricao = planoReader.GetString(planoReader.GetOrdinal("Descricao")),
            Divisao = planoReader.GetInt32(planoReader.GetOrdinal("Divisao")),
            Nivel = planoReader.GetInt32(planoReader.GetOrdinal("Nivel")),
            FrequenciaSemanal = planoReader.GetInt32(planoReader.GetOrdinal("FrequenciaSemanal")),
            UsuarioId = planoReader.GetGuid(planoReader.GetOrdinal("UsuarioId")),
            CriadoEm = planoReader.GetDateTime(planoReader.GetOrdinal("CriadoEm")),
            AtualizadoEm = planoReader.IsDBNull(planoReader.GetOrdinal("AtualizadoEm"))
                ? null : planoReader.GetDateTime(planoReader.GetOrdinal("AtualizadoEm"))
        };

        planoReader.Close();

        // Carrega dias e exercicios
        const string diasSql = @"
            SELECT d.Id, d.Nome, d.Ordem,
                   e.Id AS ExId, e.Nome AS ExNome, e.Descricao AS ExDescricao,
                   e.GrupoMuscular, e.Series, e.Repeticoes, e.Carga,
                   e.Observacoes, e.Ordem AS ExOrdem
            FROM DiaTreino d
            LEFT JOIN Exercicio e ON e.DiaTreinoId = d.Id
            WHERE d.PlanoTreinoId = @PlanoTreinoId
            ORDER BY d.Ordem, e.Ordem";

        await using var diasCommand = new SqlCommand(diasSql, connection);
        diasCommand.Parameters.AddWithValue("@PlanoTreinoId", id);

        await using var diasReader = await diasCommand.ExecuteReaderAsync();

        var diasMap = new Dictionary<Guid, DiaTreinoDto>();

        while (await diasReader.ReadAsync())
        {
            var diaId = diasReader.GetGuid(diasReader.GetOrdinal("Id"));

            if (!diasMap.TryGetValue(diaId, out var dia))
            {
                dia = new DiaTreinoDto
                {
                    Id = diaId,
                    Nome = diasReader.GetString(diasReader.GetOrdinal("Nome")),
                    Ordem = diasReader.GetInt32(diasReader.GetOrdinal("Ordem")),
                    Exercicios = new List<ExercicioDto>()
                };
                diasMap[diaId] = dia;
            }

            if (!diasReader.IsDBNull(diasReader.GetOrdinal("ExId")))
            {
                dia.Exercicios.Add(new ExercicioDto
                {
                    Id = diasReader.GetGuid(diasReader.GetOrdinal("ExId")),
                    Nome = diasReader.GetString(diasReader.GetOrdinal("ExNome")),
                    Descricao = diasReader.IsDBNull(diasReader.GetOrdinal("ExDescricao"))
                        ? null : diasReader.GetString(diasReader.GetOrdinal("ExDescricao")),
                    GrupoMuscular = diasReader.GetInt32(diasReader.GetOrdinal("GrupoMuscular")),
                    Series = diasReader.GetInt32(diasReader.GetOrdinal("Series")),
                    Repeticoes = diasReader.GetInt32(diasReader.GetOrdinal("Repeticoes")),
                    Carga = diasReader.IsDBNull(diasReader.GetOrdinal("Carga"))
                        ? null : (double)diasReader.GetDouble(diasReader.GetOrdinal("Carga")),
                    Observacoes = diasReader.IsDBNull(diasReader.GetOrdinal("Observacoes"))
                        ? null : diasReader.GetString(diasReader.GetOrdinal("Observacoes")),
                    Ordem = diasReader.GetInt32(diasReader.GetOrdinal("ExOrdem"))
                });
            }
        }

        plano.Dias = diasMap.Values.OrderBy(d => d.Ordem).ToList();
        return plano;
    }

    public async Task<List<PlanoTreinoDto>> ListarPorUsuarioAsync(Guid usuarioId)
    {
        var planos = new List<PlanoTreinoDto>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = @"
            SELECT Id FROM PlanoTreino
            WHERE UsuarioId = @UsuarioId
            ORDER BY CriadoEm DESC";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@UsuarioId", usuarioId);

        await using var reader = await command.ExecuteReaderAsync();

        var ids = new List<Guid>();
        while (await reader.ReadAsync())
            ids.Add(reader.GetGuid(reader.GetOrdinal("Id")));

        reader.Close();

        foreach (var id in ids)
        {
            var plano = await ObterPorIdAsync(id);
            if (plano is not null)
                planos.Add(plano);
        }

        return planos;
    }

    public async Task<PlanoTreinoDto?> CriarAsync(Guid usuarioId, PlanoTreinoRequest request)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = connection.BeginTransaction();

        try
        {
            var planoId = Guid.NewGuid();

            const string insertPlanoSql = @"
                INSERT INTO PlanoTreino (Id, UsuarioId, Nome, Descricao, Divisao, Nivel, FrequenciaSemanal, CriadoEm)
                VALUES (@Id, @UsuarioId, @Nome, @Descricao, @Divisao, @Nivel, @FrequenciaSemanal, GETUTCDATE())";

            await using var insertCommand = new SqlCommand(insertPlanoSql, connection, transaction);
            insertCommand.Parameters.AddWithValue("@Id", planoId);
            insertCommand.Parameters.AddWithValue("@UsuarioId", usuarioId);
            insertCommand.Parameters.AddWithValue("@Nome", request.Nome);
            insertCommand.Parameters.AddWithValue("@Descricao", request.Descricao);
            insertCommand.Parameters.AddWithValue("@Divisao", request.Divisao);
            insertCommand.Parameters.AddWithValue("@Nivel", request.Nivel);
            insertCommand.Parameters.AddWithValue("@FrequenciaSemanal", request.FrequenciaSemanal);
            await insertCommand.ExecuteNonQueryAsync();

            // Insere dias e exercícios
            const string insertDiaSql = @"
                INSERT INTO DiaTreino (Id, PlanoTreinoId, Nome, Ordem)
                VALUES (@Id, @PlanoTreinoId, @Nome, @Ordem)";

            const string insertExercicioSql = @"
                INSERT INTO Exercicio (Id, DiaTreinoId, Nome, Descricao, GrupoMuscular, Series, Repeticoes, Carga, Observacoes, Ordem)
                VALUES (@Id, @DiaTreinoId, @Nome, @Descricao, @GrupoMuscular, @Series, @Repeticoes, @Carga, @Observacoes, @Ordem)";

            foreach (var dia in request.Dias)
            {
                var diaId = Guid.NewGuid();
                await using var diaCmd = new SqlCommand(insertDiaSql, connection, transaction);
                diaCmd.Parameters.AddWithValue("@Id", diaId);
                diaCmd.Parameters.AddWithValue("@PlanoTreinoId", planoId);
                diaCmd.Parameters.AddWithValue("@Nome", dia.Nome);
                diaCmd.Parameters.AddWithValue("@Ordem", dia.Ordem);
                await diaCmd.ExecuteNonQueryAsync();

                foreach (var ex in dia.Exercicios)
                {
                    await using var exCmd = new SqlCommand(insertExercicioSql, connection, transaction);
                    exCmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    exCmd.Parameters.AddWithValue("@DiaTreinoId", diaId);
                    exCmd.Parameters.AddWithValue("@Nome", ex.Nome);
                    exCmd.Parameters.AddWithValue("@Descricao", (object?)ex.Descricao ?? DBNull.Value);
                    exCmd.Parameters.AddWithValue("@GrupoMuscular", ex.GrupoMuscular);
                    exCmd.Parameters.AddWithValue("@Series", ex.Series);
                    exCmd.Parameters.AddWithValue("@Repeticoes", ex.Repeticoes);
                    exCmd.Parameters.AddWithValue("@Carga", (object?)ex.Carga ?? DBNull.Value);
                    exCmd.Parameters.AddWithValue("@Observacoes", (object?)ex.Observacoes ?? DBNull.Value);
                    exCmd.Parameters.AddWithValue("@Ordem", ex.Ordem);
                    await exCmd.ExecuteNonQueryAsync();
                }
            }

            await transaction.CommitAsync();

            return await ObterPorIdAsync(planoId);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> AtualizarAsync(Guid id, PlanoTreinoRequest request)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = @"
            UPDATE PlanoTreino
            SET Nome = @Nome,
                Descricao = @Descricao,
                Divisao = @Divisao,
                Nivel = @Nivel,
                FrequenciaSemanal = @FrequenciaSemanal,
                AtualizadoEm = GETUTCDATE()
            WHERE Id = @Id";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Nome", request.Nome);
        command.Parameters.AddWithValue("@Descricao", request.Descricao);
        command.Parameters.AddWithValue("@Divisao", request.Divisao);
        command.Parameters.AddWithValue("@Nivel", request.Nivel);
        command.Parameters.AddWithValue("@FrequenciaSemanal", request.FrequenciaSemanal);

        var linhas = await command.ExecuteNonQueryAsync();
        return linhas > 0;
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = "DELETE FROM PlanoTreino WHERE Id = @Id";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        var linhas = await command.ExecuteNonQueryAsync();
        return linhas > 0;
    }
}
