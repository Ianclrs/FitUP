using System.Data;
using FitUP.WebApi.DTOs;
using Microsoft.Data.SqlClient;

namespace FitUP.WebApi.Service;

public class UsuarioService : IUsuarioService
{
    private readonly string _connectionString;

    public UsuarioService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<UsuarioDto?> ObterPorIdAsync(Guid id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = @"
            SELECT Id, Nome, Sobrenome, Email, Telefone, CPF, DataNascimento,
                   CriadoEm, UltimoLoginEm, Ativo
            FROM Usuario
            WHERE Id = @Id";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new UsuarioDto
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            Nome = reader.GetString(reader.GetOrdinal("Nome")),
            Sobrenome = reader.GetString(reader.GetOrdinal("Sobrenome")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            Telefone = reader.IsDBNull(reader.GetOrdinal("Telefone")) ? null : reader.GetString(reader.GetOrdinal("Telefone")),
            CPF = reader.IsDBNull(reader.GetOrdinal("CPF")) ? null : reader.GetString(reader.GetOrdinal("CPF")),
            DataNascimento = reader.IsDBNull(reader.GetOrdinal("DataNascimento")) ? null : reader.GetDateTime(reader.GetOrdinal("DataNascimento")),
            CriadoEm = reader.GetDateTime(reader.GetOrdinal("CriadoEm")),
            UltimoLoginEm = reader.IsDBNull(reader.GetOrdinal("UltimoLoginEm")) ? null : reader.GetDateTime(reader.GetOrdinal("UltimoLoginEm")),
            Ativo = reader.GetBoolean(reader.GetOrdinal("Ativo"))
        };
    }

    public async Task<bool> AtualizarAsync(Guid id, UsuarioUpdateRequest request)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = @"
            UPDATE Usuario
            SET Nome = COALESCE(@Nome, Nome),
                Sobrenome = COALESCE(@Sobrenome, Sobrenome),
                Email = COALESCE(@Email, Email),
                Telefone = COALESCE(@Telefone, Telefone),
                CPF = COALESCE(@CPF, CPF),
                DataNascimento = COALESCE(@DataNascimento, DataNascimento)
            WHERE Id = @Id";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Nome", request.Nome);
        command.Parameters.AddWithValue("@Sobrenome", request.Sobrenome);
        command.Parameters.AddWithValue("@Email", (object?)request.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("@Telefone", (object?)request.Telefone ?? DBNull.Value);
        command.Parameters.AddWithValue("@CPF", (object?)request.CPF ?? DBNull.Value);
        command.Parameters.AddWithValue("@DataNascimento", (object?)request.DataNascimento ?? DBNull.Value);

        var linhas = await command.ExecuteNonQueryAsync();
        return linhas > 0;
    }

    public async Task<bool> AlterarSenhaAsync(Guid id, AlterarSenhaRequest request)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Busca senha atual
        const string selectSql = "SELECT SenhaHash FROM Usuario WHERE Id = @Id";
        await using var selectCommand = new SqlCommand(selectSql, connection);
        selectCommand.Parameters.AddWithValue("@Id", id);

        var senhaHashAtual = await selectCommand.ExecuteScalarAsync() as string;
        if (senhaHashAtual is null)
            return false;

        if (!BCrypt.Net.BCrypt.Verify(request.SenhaAtual, senhaHashAtual))
            return false;

        var novaSenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);

        const string updateSql = "UPDATE Usuario SET SenhaHash = @SenhaHash WHERE Id = @Id";
        await using var updateCommand = new SqlCommand(updateSql, connection);
        updateCommand.Parameters.AddWithValue("@SenhaHash", novaSenhaHash);
        updateCommand.Parameters.AddWithValue("@Id", id);

        var linhas = await updateCommand.ExecuteNonQueryAsync();
        return linhas > 0;
    }

    public async Task<bool> DesativarAsync(Guid id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = "UPDATE Usuario SET Ativo = 0 WHERE Id = @Id";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        var linhas = await command.ExecuteNonQueryAsync();
        return linhas > 0;
    }
}
