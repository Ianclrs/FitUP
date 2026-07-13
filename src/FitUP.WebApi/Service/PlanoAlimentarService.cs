using System.Data;
using FitUP.WebApi.DTOs;
using Microsoft.Data.SqlClient;

namespace FitUP.WebApi.Service;

public class PlanoAlimentarService : IPlanoAlimentarService
{
    private readonly string _connectionString;

    public PlanoAlimentarService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<PlanoAlimentarDto?> ObterPorIdAsync(Guid id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string planoSql = @"
            SELECT Id, Nome, Objetivo, Descricao, UsuarioId, CriadoEm, AtualizadoEm
            FROM PlanoAlimentar WHERE Id = @Id";

        await using var planoCommand = new SqlCommand(planoSql, connection);
        planoCommand.Parameters.AddWithValue("@Id", id);

        await using var planoReader = await planoCommand.ExecuteReaderAsync();
        if (!await planoReader.ReadAsync())
            return null;

        var plano = new PlanoAlimentarDto
        {
            Id = planoReader.GetGuid(planoReader.GetOrdinal("Id")),
            Nome = planoReader.GetString(planoReader.GetOrdinal("Nome")),
            Objetivo = planoReader.GetInt32(planoReader.GetOrdinal("Objetivo")),
            Descricao = planoReader.GetString(planoReader.GetOrdinal("Descricao")),
            UsuarioId = planoReader.GetGuid(planoReader.GetOrdinal("UsuarioId")),
            CriadoEm = planoReader.GetDateTime(planoReader.GetOrdinal("CriadoEm")),
            AtualizadoEm = planoReader.IsDBNull(planoReader.GetOrdinal("AtualizadoEm"))
                ? null : planoReader.GetDateTime(planoReader.GetOrdinal("AtualizadoEm"))
        };

        planoReader.Close();

        // Carrega refeicoes e alimentos
        const string refeicoesSql = @"
            SELECT r.Id, r.Nome, r.HorarioSugerido, r.Ordem,
                   r.TotalProteina, r.TotalCarboidrato, r.TotalGordura,
                   r.TotalFibra, r.TotalCalorias,
                   a.Id AS AlId, a.Nome AS AlNome, a.Quantidade, a.UnidadeMedida,
                   a.Proteina AS AlProteina, a.Carboidrato AS AlCarboidrato,
                   a.Gordura AS AlGordura, a.Fibra AS AlFibra,
                   a.Calorias AS AlCalorias, a.Observacoes
            FROM Refeicao r
            LEFT JOIN Alimento a ON a.RefeicaoId = r.Id
            WHERE r.PlanoAlimentarId = @PlanoAlimentarId
            ORDER BY r.Ordem, a.Id";

        await using var refeicoesCommand = new SqlCommand(refeicoesSql, connection);
        refeicoesCommand.Parameters.AddWithValue("@PlanoAlimentarId", id);

        await using var refeicoesReader = await refeicoesCommand.ExecuteReaderAsync();

        var refeicoesMap = new Dictionary<Guid, RefeicaoDto>();

        while (await refeicoesReader.ReadAsync())
        {
            var refeicaoId = refeicoesReader.GetGuid(refeicoesReader.GetOrdinal("Id"));

            if (!refeicoesMap.TryGetValue(refeicaoId, out var refeicao))
            {
                refeicao = new RefeicaoDto
                {
                    Id = refeicaoId,
                    Nome = refeicoesReader.GetString(refeicoesReader.GetOrdinal("Nome")),
                    HorarioSugerido = refeicoesReader.IsDBNull(refeicoesReader.GetOrdinal("HorarioSugerido"))
                        ? null : refeicoesReader.GetTimeSpan(refeicoesReader.GetOrdinal("HorarioSugerido")),
                    Ordem = refeicoesReader.GetInt32(refeicoesReader.GetOrdinal("Ordem")),
                    TotalProteina = refeicoesReader.GetDouble(refeicoesReader.GetOrdinal("TotalProteina")),
                    TotalCarboidrato = refeicoesReader.GetDouble(refeicoesReader.GetOrdinal("TotalCarboidrato")),
                    TotalGordura = refeicoesReader.GetDouble(refeicoesReader.GetOrdinal("TotalGordura")),
                    TotalFibra = refeicoesReader.GetDouble(refeicoesReader.GetOrdinal("TotalFibra")),
                    TotalCalorias = refeicoesReader.GetDouble(refeicoesReader.GetOrdinal("TotalCalorias")),
                    Alimentos = new List<AlimentoDto>()
                };
                refeicoesMap[refeicaoId] = refeicao;
            }

            if (!refeicoesReader.IsDBNull(refeicoesReader.GetOrdinal("AlId")))
            {
                refeicao.Alimentos.Add(new AlimentoDto
                {
                    Id = refeicoesReader.GetGuid(refeicoesReader.GetOrdinal("AlId")),
                    Nome = refeicoesReader.GetString(refeicoesReader.GetOrdinal("AlNome")),
                    Quantidade = refeicoesReader.GetDouble(refeicoesReader.GetOrdinal("Quantidade")),
                    UnidadeMedida = refeicoesReader.GetString(refeicoesReader.GetOrdinal("UnidadeMedida")),
                    Proteina = refeicoesReader.GetDouble(refeicoesReader.GetOrdinal("AlProteina")),
                    Carboidrato = refeicoesReader.GetDouble(refeicoesReader.GetOrdinal("AlCarboidrato")),
                    Gordura = refeicoesReader.GetDouble(refeicoesReader.GetOrdinal("AlGordura")),
                    Fibra = refeicoesReader.GetDouble(refeicoesReader.GetOrdinal("AlFibra")),
                    Calorias = refeicoesReader.GetDouble(refeicoesReader.GetOrdinal("AlCalorias")),
                    Observacoes = refeicoesReader.IsDBNull(refeicoesReader.GetOrdinal("Observacoes"))
                        ? null : refeicoesReader.GetString(refeicoesReader.GetOrdinal("Observacoes"))
                });
            }
        }

        plano.Refeicoes = refeicoesMap.Values.OrderBy(r => r.Ordem).ToList();
        return plano;
    }

    public async Task<List<PlanoAlimentarDto>> ListarPorUsuarioAsync(Guid usuarioId)
    {
        var planos = new List<PlanoAlimentarDto>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = @"
            SELECT Id FROM PlanoAlimentar
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

    public async Task<PlanoAlimentarDto?> CriarAsync(Guid usuarioId, PlanoAlimentarRequest request)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        try
        {
            var planoId = Guid.NewGuid();

            const string insertSql = @"
                INSERT INTO PlanoAlimentar (Id, UsuarioId, Nome, Objetivo, Descricao, CriadoEm)
                VALUES (@Id, @UsuarioId, @Nome, @Objetivo, @Descricao, GETUTCDATE())";

            await using var command = new SqlCommand(insertSql, connection, transaction);
            command.Parameters.AddWithValue("@Id", planoId);
            command.Parameters.AddWithValue("@UsuarioId", usuarioId);
            command.Parameters.AddWithValue("@Nome", request.Nome);
            command.Parameters.AddWithValue("@Objetivo", request.Objetivo);
            command.Parameters.AddWithValue("@Descricao", request.Descricao);
            await command.ExecuteNonQueryAsync();

            // Inserir refeições e alimentos aninhados, se fornecidos
            if (request.Refeicoes is { Count: > 0 })
            {
                const string refeicaoSql = @"
                    INSERT INTO Refeicao (Id, PlanoAlimentarId, Nome, HorarioSugerido, Ordem,
                        TotalProteina, TotalCarboidrato, TotalGordura, TotalFibra, TotalCalorias)
                    VALUES (@Id, @PlanoAlimentarId, @Nome, @HorarioSugerido, @Ordem,
                        @TotalProteina, @TotalCarboidrato, @TotalGordura, @TotalFibra, @TotalCalorias)";

                const string alimentoSql = @"
                    INSERT INTO Alimento (Id, RefeicaoId, Nome, Quantidade, UnidadeMedida,
                        Proteina, Carboidrato, Gordura, Fibra, Calorias, Observacoes)
                    VALUES (@Id, @RefeicaoId, @Nome, @Quantidade, @UnidadeMedida,
                        @Proteina, @Carboidrato, @Gordura, @Fibra, @Calorias, @Observacoes)";

                foreach (var refeicaoRequest in request.Refeicoes)
                {
                    var refeicaoId = Guid.NewGuid();

                    await using var refCmd = new SqlCommand(refeicaoSql, connection, transaction);
                    refCmd.Parameters.AddWithValue("@Id", refeicaoId);
                    refCmd.Parameters.AddWithValue("@PlanoAlimentarId", planoId);
                    refCmd.Parameters.AddWithValue("@Nome", refeicaoRequest.Nome);
                    refCmd.Parameters.AddWithValue("@HorarioSugerido",
                        refeicaoRequest.HorarioSugerido.HasValue ? refeicaoRequest.HorarioSugerido.Value : DBNull.Value);
                    refCmd.Parameters.AddWithValue("@Ordem", refeicaoRequest.Ordem);
                    refCmd.Parameters.AddWithValue("@TotalProteina", refeicaoRequest.TotalProteina);
                    refCmd.Parameters.AddWithValue("@TotalCarboidrato", refeicaoRequest.TotalCarboidrato);
                    refCmd.Parameters.AddWithValue("@TotalGordura", refeicaoRequest.TotalGordura);
                    refCmd.Parameters.AddWithValue("@TotalFibra", refeicaoRequest.TotalFibra);
                    refCmd.Parameters.AddWithValue("@TotalCalorias", refeicaoRequest.TotalCalorias);
                    await refCmd.ExecuteNonQueryAsync();

                    if (refeicaoRequest.Alimentos is { Count: > 0 })
                    {
                        foreach (var alimentoRequest in refeicaoRequest.Alimentos)
                        {
                            await using var aliCmd = new SqlCommand(alimentoSql, connection, transaction);
                            aliCmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                            aliCmd.Parameters.AddWithValue("@RefeicaoId", refeicaoId);
                            aliCmd.Parameters.AddWithValue("@Nome", alimentoRequest.Nome);
                            aliCmd.Parameters.AddWithValue("@Quantidade", alimentoRequest.Quantidade);
                            aliCmd.Parameters.AddWithValue("@UnidadeMedida", alimentoRequest.UnidadeMedida);
                            aliCmd.Parameters.AddWithValue("@Proteina", alimentoRequest.Proteina);
                            aliCmd.Parameters.AddWithValue("@Carboidrato", alimentoRequest.Carboidrato);
                            aliCmd.Parameters.AddWithValue("@Gordura", alimentoRequest.Gordura);
                            aliCmd.Parameters.AddWithValue("@Fibra", alimentoRequest.Fibra);
                            aliCmd.Parameters.AddWithValue("@Calorias", alimentoRequest.Calorias);
                            aliCmd.Parameters.AddWithValue("@Observacoes",
                                alimentoRequest.Observacoes is not null ? alimentoRequest.Observacoes : DBNull.Value);
                            await aliCmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            }

            transaction.Commit();

            return await ObterPorIdAsync(planoId);
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> AtualizarAsync(Guid id, PlanoAlimentarRequest request)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = @"
            UPDATE PlanoAlimentar
            SET Nome = @Nome,
                Objetivo = @Objetivo,
                Descricao = @Descricao,
                AtualizadoEm = GETUTCDATE()
            WHERE Id = @Id";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Nome", request.Nome);
        command.Parameters.AddWithValue("@Objetivo", request.Objetivo);
        command.Parameters.AddWithValue("@Descricao", request.Descricao);

        var linhas = await command.ExecuteNonQueryAsync();
        return linhas > 0;
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = "DELETE FROM PlanoAlimentar WHERE Id = @Id";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        var linhas = await command.ExecuteNonQueryAsync();
        return linhas > 0;
    }
}
