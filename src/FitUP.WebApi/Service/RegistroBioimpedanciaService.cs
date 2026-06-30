using System.Data;
using FitUP.WebApi.DTOs;
using Microsoft.Data.SqlClient;

namespace FitUP.WebApi.Service;

public class RegistroBioimpedanciaService : IRegistroBioimpedanciaService
{
    private readonly string _connectionString;

    public RegistroBioimpedanciaService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<RegistroBioimpedanciaDto?> ObterPorIdAsync(Guid id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = @"
            SELECT Id, UsuarioId, DataRegistro, Peso, Altura,
                   MassaMagra, MassaGorda, PercentualGordura, MassaMuscular,
                   AguaCorporal, TaxaMetabolicaBasal, IdadeMetabolica,
                   CircunferenciaCintura, CircunferenciaQuadril, RelacaoCinturaQuadril,
                   Observacoes
            FROM RegistroBioimpedancia
            WHERE Id = @Id";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return MapRegistro(reader);
    }

    public async Task<List<RegistroBioimpedanciaDto>> ListarPorUsuarioAsync(Guid usuarioId)
    {
        var registros = new List<RegistroBioimpedanciaDto>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = @"
            SELECT Id, UsuarioId, DataRegistro, Peso, Altura,
                   MassaMagra, MassaGorda, PercentualGordura, MassaMuscular,
                   AguaCorporal, TaxaMetabolicaBasal, IdadeMetabolica,
                   CircunferenciaCintura, CircunferenciaQuadril, RelacaoCinturaQuadril,
                   Observacoes
            FROM RegistroBioimpedancia
            WHERE UsuarioId = @UsuarioId
            ORDER BY DataRegistro DESC";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@UsuarioId", usuarioId);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            registros.Add(MapRegistro(reader));

        return registros;
    }

    public async Task<RegistroBioimpedanciaDto?> CriarAsync(Guid usuarioId, RegistroBioimpedanciaRequest request)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var id = Guid.NewGuid();

        const string sql = @"
            INSERT INTO RegistroBioimpedancia (
                Id, UsuarioId, DataRegistro, Peso, Altura,
                MassaMagra, MassaGorda, PercentualGordura, MassaMuscular,
                AguaCorporal, TaxaMetabolicaBasal, IdadeMetabolica,
                CircunferenciaCintura, CircunferenciaQuadril, RelacaoCinturaQuadril,
                Observacoes
            ) VALUES (
                @Id, @UsuarioId, @DataRegistro, @Peso, @Altura,
                @MassaMagra, @MassaGorda, @PercentualGordura, @MassaMuscular,
                @AguaCorporal, @TaxaMetabolicaBasal, @IdadeMetabolica,
                @CircunferenciaCintura, @CircunferenciaQuadril, @RelacaoCinturaQuadril,
                @Observacoes
            )";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@UsuarioId", usuarioId);
        command.Parameters.AddWithValue("@DataRegistro", request.DataRegistro);
        command.Parameters.AddWithValue("@Peso", request.Peso);
        command.Parameters.AddWithValue("@Altura", request.Altura);
        command.Parameters.AddWithValue("@MassaMagra", (object?)request.MassaMagra ?? DBNull.Value);
        command.Parameters.AddWithValue("@MassaGorda", (object?)request.MassaGorda ?? DBNull.Value);
        command.Parameters.AddWithValue("@PercentualGordura", (object?)request.PercentualGordura ?? DBNull.Value);
        command.Parameters.AddWithValue("@MassaMuscular", (object?)request.MassaMuscular ?? DBNull.Value);
        command.Parameters.AddWithValue("@AguaCorporal", (object?)request.AguaCorporal ?? DBNull.Value);
        command.Parameters.AddWithValue("@TaxaMetabolicaBasal", (object?)request.TaxaMetabolicaBasal ?? DBNull.Value);
        command.Parameters.AddWithValue("@IdadeMetabolica", (object?)request.IdadeMetabolica ?? DBNull.Value);
        command.Parameters.AddWithValue("@CircunferenciaCintura", (object?)request.CircunferenciaCintura ?? DBNull.Value);
        command.Parameters.AddWithValue("@CircunferenciaQuadril", (object?)request.CircunferenciaQuadril ?? DBNull.Value);
        command.Parameters.AddWithValue("@RelacaoCinturaQuadril", (object?)request.RelacaoCinturaQuadril ?? DBNull.Value);
        command.Parameters.AddWithValue("@Observacoes", (object?)request.Observacoes ?? DBNull.Value);
        await command.ExecuteNonQueryAsync();

        return await ObterPorIdAsync(id);
    }

    public async Task<bool> AtualizarAsync(Guid id, RegistroBioimpedanciaRequest request)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = @"
            UPDATE RegistroBioimpedancia
            SET DataRegistro = @DataRegistro,
                Peso = @Peso,
                Altura = @Altura,
                MassaMagra = @MassaMagra,
                MassaGorda = @MassaGorda,
                PercentualGordura = @PercentualGordura,
                MassaMuscular = @MassaMuscular,
                AguaCorporal = @AguaCorporal,
                TaxaMetabolicaBasal = @TaxaMetabolicaBasal,
                IdadeMetabolica = @IdadeMetabolica,
                CircunferenciaCintura = @CircunferenciaCintura,
                CircunferenciaQuadril = @CircunferenciaQuadril,
                RelacaoCinturaQuadril = @RelacaoCinturaQuadril,
                Observacoes = @Observacoes
            WHERE Id = @Id";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@DataRegistro", request.DataRegistro);
        command.Parameters.AddWithValue("@Peso", request.Peso);
        command.Parameters.AddWithValue("@Altura", request.Altura);
        command.Parameters.AddWithValue("@MassaMagra", (object?)request.MassaMagra ?? DBNull.Value);
        command.Parameters.AddWithValue("@MassaGorda", (object?)request.MassaGorda ?? DBNull.Value);
        command.Parameters.AddWithValue("@PercentualGordura", (object?)request.PercentualGordura ?? DBNull.Value);
        command.Parameters.AddWithValue("@MassaMuscular", (object?)request.MassaMuscular ?? DBNull.Value);
        command.Parameters.AddWithValue("@AguaCorporal", (object?)request.AguaCorporal ?? DBNull.Value);
        command.Parameters.AddWithValue("@TaxaMetabolicaBasal", (object?)request.TaxaMetabolicaBasal ?? DBNull.Value);
        command.Parameters.AddWithValue("@IdadeMetabolica", (object?)request.IdadeMetabolica ?? DBNull.Value);
        command.Parameters.AddWithValue("@CircunferenciaCintura", (object?)request.CircunferenciaCintura ?? DBNull.Value);
        command.Parameters.AddWithValue("@CircunferenciaQuadril", (object?)request.CircunferenciaQuadril ?? DBNull.Value);
        command.Parameters.AddWithValue("@RelacaoCinturaQuadril", (object?)request.RelacaoCinturaQuadril ?? DBNull.Value);
        command.Parameters.AddWithValue("@Observacoes", (object?)request.Observacoes ?? DBNull.Value);

        var linhas = await command.ExecuteNonQueryAsync();
        return linhas > 0;
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = "DELETE FROM RegistroBioimpedancia WHERE Id = @Id";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        var linhas = await command.ExecuteNonQueryAsync();
        return linhas > 0;
    }

    private static RegistroBioimpedanciaDto MapRegistro(SqlDataReader reader)
    {
        return new RegistroBioimpedanciaDto
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            UsuarioId = reader.GetGuid(reader.GetOrdinal("UsuarioId")),
            DataRegistro = reader.GetDateTime(reader.GetOrdinal("DataRegistro")),
            Peso = reader.GetDouble(reader.GetOrdinal("Peso")),
            Altura = reader.GetDouble(reader.GetOrdinal("Altura")),
            MassaMagra = reader.IsDBNull(reader.GetOrdinal("MassaMagra")) ? null : reader.GetDouble(reader.GetOrdinal("MassaMagra")),
            MassaGorda = reader.IsDBNull(reader.GetOrdinal("MassaGorda")) ? null : reader.GetDouble(reader.GetOrdinal("MassaGorda")),
            PercentualGordura = reader.IsDBNull(reader.GetOrdinal("PercentualGordura")) ? null : reader.GetDouble(reader.GetOrdinal("PercentualGordura")),
            MassaMuscular = reader.IsDBNull(reader.GetOrdinal("MassaMuscular")) ? null : reader.GetDouble(reader.GetOrdinal("MassaMuscular")),
            AguaCorporal = reader.IsDBNull(reader.GetOrdinal("AguaCorporal")) ? null : reader.GetDouble(reader.GetOrdinal("AguaCorporal")),
            TaxaMetabolicaBasal = reader.IsDBNull(reader.GetOrdinal("TaxaMetabolicaBasal")) ? null : reader.GetDouble(reader.GetOrdinal("TaxaMetabolicaBasal")),
            IdadeMetabolica = reader.IsDBNull(reader.GetOrdinal("IdadeMetabolica")) ? null : reader.GetDouble(reader.GetOrdinal("IdadeMetabolica")),
            CircunferenciaCintura = reader.IsDBNull(reader.GetOrdinal("CircunferenciaCintura")) ? null : reader.GetDouble(reader.GetOrdinal("CircunferenciaCintura")),
            CircunferenciaQuadril = reader.IsDBNull(reader.GetOrdinal("CircunferenciaQuadril")) ? null : reader.GetDouble(reader.GetOrdinal("CircunferenciaQuadril")),
            RelacaoCinturaQuadril = reader.IsDBNull(reader.GetOrdinal("RelacaoCinturaQuadril")) ? null : reader.GetDouble(reader.GetOrdinal("RelacaoCinturaQuadril")),
            Observacoes = reader.IsDBNull(reader.GetOrdinal("Observacoes")) ? null : reader.GetString(reader.GetOrdinal("Observacoes"))
        };
    }
}
