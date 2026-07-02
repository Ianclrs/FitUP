using System.Data;
using FitUP.WebApi.DTOs;
using FitUP.WebApi.Models;
using Microsoft.Data.SqlClient;

namespace FitUP.WebApi.Service;

public class AuthService : IAuthService
{
    private readonly string _connectionString;
    private readonly ITokenService _tokenService;

    public AuthService(IConfiguration configuration, ITokenService tokenService)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = @"
            SELECT Id, Nome, Sobrenome, Email, SenhaHash, Ativo,
                   RefreshToken, RefreshTokenExpiraEm
            FROM Usuario
            WHERE Email = @Email";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Email", request.Email);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        var usuario = new Usuario
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            Nome = reader.GetString(reader.GetOrdinal("Nome")),
            Sobrenome = reader.GetString(reader.GetOrdinal("Sobrenome")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            SenhaHash = reader.GetString(reader.GetOrdinal("SenhaHash")),
            Ativo = reader.GetBoolean(reader.GetOrdinal("Ativo")),
            RefreshToken = reader.IsDBNull(reader.GetOrdinal("RefreshToken"))
                ? null : reader.GetString(reader.GetOrdinal("RefreshToken")),
            RefreshTokenExpiraEm = reader.IsDBNull(reader.GetOrdinal("RefreshTokenExpiraEm"))
                ? null : reader.GetDateTime(reader.GetOrdinal("RefreshTokenExpiraEm"))
        };

        if (!usuario.Ativo)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            return null;

        // Gera novos tokens
        var (token, expiraEm) = _tokenService.GerarTokenJwt(usuario);
        var refreshToken = _tokenService.GerarRefreshToken();
        var refreshTokenExpiraEm = DateTime.UtcNow.AddDays(7);

        reader.Close();

        // Atualiza refresh token e ultimo login no banco
        const string updateSql = @"
            UPDATE Usuario
            SET RefreshToken = @RefreshToken,
                RefreshTokenExpiraEm = @RefreshTokenExpiraEm,
                UltimoLoginEm = GETUTCDATE()
            WHERE Id = @Id";

        await using var updateCommand = new SqlCommand(updateSql, connection);
        updateCommand.Parameters.AddWithValue("@RefreshToken", refreshToken);
        updateCommand.Parameters.AddWithValue("@RefreshTokenExpiraEm", refreshTokenExpiraEm);
        updateCommand.Parameters.AddWithValue("@Id", usuario.Id);
        await updateCommand.ExecuteNonQueryAsync();

        return new AuthResponse
        {
            UsuarioId = usuario.Id,
            Nome = $"{usuario.Nome} {usuario.Sobrenome}",
            Email = usuario.Email,
            Token = token,
            RefreshToken = refreshToken,
            ExpiraEm = expiraEm
        };
    }

    public async Task<AuthResponse?> RegistrarAsync(RegistroRequest request)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Verifica se email já existe
        const string checkSql = "SELECT COUNT(1) FROM Usuario WHERE Email = @Email";
        await using var checkCommand = new SqlCommand(checkSql, connection);
        checkCommand.Parameters.AddWithValue("@Email", request.Email);

        var exists = (int)(await checkCommand.ExecuteScalarAsync() ?? 0);
        if (exists > 0)
            return null;

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
        var usuarioId = Guid.NewGuid();

        const string insertSql = @"
            INSERT INTO Usuario (Id, Nome, Sobrenome, Email, SenhaHash, Telefone, CPF, DataNascimento, CriadoEm, Ativo)
            VALUES (@Id, @Nome, @Sobrenome, @Email, @SenhaHash, @Telefone, @CPF, @DataNascimento, GETUTCDATE(), 1)";

        await using var insertCommand = new SqlCommand(insertSql, connection);
        insertCommand.Parameters.AddWithValue("@Id", usuarioId);
        insertCommand.Parameters.AddWithValue("@Nome", request.Nome);
        insertCommand.Parameters.AddWithValue("@Sobrenome", request.Sobrenome);
        insertCommand.Parameters.AddWithValue("@Email", request.Email);
        insertCommand.Parameters.AddWithValue("@SenhaHash", senhaHash);
        insertCommand.Parameters.AddWithValue("@Telefone", (object?)request.Telefone ?? DBNull.Value);
        insertCommand.Parameters.AddWithValue("@CPF", (object?)request.CPF ?? DBNull.Value);
        insertCommand.Parameters.AddWithValue("@DataNascimento", (object?)request.DataNascimento ?? DBNull.Value);
        await insertCommand.ExecuteNonQueryAsync();

        var usuario = new Usuario
        {
            Id = usuarioId,
            Nome = request.Nome,
            Sobrenome = request.Sobrenome,
            Email = request.Email,
            SenhaHash = senhaHash,
            Ativo = true
        };

        var (token, expiraEm) = _tokenService.GerarTokenJwt(usuario);
        var refreshToken = _tokenService.GerarRefreshToken();
        var refreshTokenExpiraEm = DateTime.UtcNow.AddDays(7);

        // Salva refresh token
        const string updateSql = @"
            UPDATE Usuario SET RefreshToken = @RefreshToken, RefreshTokenExpiraEm = @ExpiraEm WHERE Id = @Id";
        await using var updateCommand = new SqlCommand(updateSql, connection);
        updateCommand.Parameters.AddWithValue("@RefreshToken", refreshToken);
        updateCommand.Parameters.AddWithValue("@ExpiraEm", refreshTokenExpiraEm);
        updateCommand.Parameters.AddWithValue("@Id", usuarioId);
        await updateCommand.ExecuteNonQueryAsync();

        return new AuthResponse
        {
            UsuarioId = usuarioId,
            Nome = $"{request.Nome} {request.Sobrenome}",
            Email = request.Email,
            Token = token,
            RefreshToken = refreshToken,
            ExpiraEm = expiraEm
        };
    }

    public async Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = @"
            SELECT Id, Nome, Sobrenome, Email, SenhaHash, Ativo
            FROM Usuario
            WHERE RefreshToken = @RefreshToken AND RefreshTokenExpiraEm > GETUTCDATE()";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@RefreshToken", request.RefreshToken);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        var usuario = new Usuario
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            Nome = reader.GetString(reader.GetOrdinal("Nome")),
            Sobrenome = reader.GetString(reader.GetOrdinal("Sobrenome")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            SenhaHash = reader.GetString(reader.GetOrdinal("SenhaHash")),
            Ativo = reader.GetBoolean(reader.GetOrdinal("Ativo"))
        };

        if (!usuario.Ativo)
            return null;

        var (token, expiraEm) = _tokenService.GerarTokenJwt(usuario);
        var novoRefreshToken = _tokenService.GerarRefreshToken();
        var refreshTokenExpiraEm = DateTime.UtcNow.AddDays(7);

        reader.Close();

        const string updateSql = @"
            UPDATE Usuario
            SET RefreshToken = @RefreshToken, RefreshTokenExpiraEm = @ExpiraEm
            WHERE Id = @Id";

        await using var updateCommand = new SqlCommand(updateSql, connection);
        updateCommand.Parameters.AddWithValue("@RefreshToken", novoRefreshToken);
        updateCommand.Parameters.AddWithValue("@ExpiraEm", refreshTokenExpiraEm);
        updateCommand.Parameters.AddWithValue("@Id", usuario.Id);
        await updateCommand.ExecuteNonQueryAsync();

        return new AuthResponse
        {
            UsuarioId = usuario.Id,
            Nome = $"{usuario.Nome} {usuario.Sobrenome}",
            Email = usuario.Email,
            Token = token,
            RefreshToken = novoRefreshToken,
            ExpiraEm = expiraEm
        };
    }
}
