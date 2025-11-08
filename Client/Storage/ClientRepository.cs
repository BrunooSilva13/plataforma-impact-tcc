using Client.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Interfaces; 

namespace Client.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ClientRepository> _logger;

        public ClientRepository(IConfiguration configuration, ILogger<ClientRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<IEnumerable<ClientModel>> GetAllClientsAsync(int page = 1, int pageSize = 10)
        {
            var clients = new List<ClientModel>();

            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                var sql = @"
                    SELECT id, name, surname, email, birthdate, isactive, created_at, updated_at
                    FROM clients
                    WHERE isactive = true
                    ORDER BY created_at DESC
                    LIMIT @PageSize OFFSET @Offset;";

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    clients.Add(new ClientModel
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1),
                        Surname = reader.GetString(2),
                        Email = reader.GetString(3),
                        Birthdate = reader.GetDateTime(4),
                        IsActive = reader.GetBoolean(5),
                        CreatedAt = reader.GetDateTime(6),
                        UpdatedAt = reader.GetDateTime(7)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar lista de clientes.");
                throw;
            }

            return clients;
        }

        public async Task<ClientModel?> GetClientByIdAsync(Guid id)
        {
            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                var sql = @"
                    SELECT id, name, surname, email, birthdate, isactive, created_at, updated_at
                    FROM clients
                    WHERE id = @Id AND isactive = true;";

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new ClientModel
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1),
                        Surname = reader.GetString(2),
                        Email = reader.GetString(3),
                        Birthdate = reader.GetDateTime(4),
                        IsActive = reader.GetBoolean(5),
                        CreatedAt = reader.GetDateTime(6),
                        UpdatedAt = reader.GetDateTime(7)
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente com Id {Id}", id);
                throw;
            }

            return null;
        }

        public async Task<Guid> AddClientAsync(ClientModel client)
        {
            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                var sql = @"
                    INSERT INTO clients (name, surname, email, birthdate, isactive, created_at, updated_at)
                    VALUES (@Name, @Surname, @Email, @Birthdate, @IsActive, NOW(), NOW())
                    RETURNING id;";

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Name", client.Name);
                cmd.Parameters.AddWithValue("@Surname", client.Surname);
                cmd.Parameters.AddWithValue("@Email", client.Email);
                cmd.Parameters.AddWithValue("@Birthdate", client.Birthdate);
                cmd.Parameters.AddWithValue("@IsActive", client.IsActive);

                var id = (Guid)await cmd.ExecuteScalarAsync();
                _logger.LogInformation("Cliente {Name} criado com Id {Id}", client.Name, id);

                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar cliente {Name}", client.Name);
                throw;
            }
        }

        public async Task<bool> UpdateClientAsync(ClientModel client)
        {
            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                var sql = @"
                    UPDATE clients
                    SET name = @Name,
                        surname = @Surname,
                        email = @Email,
                        birthdate = @Birthdate,
                        updated_at = NOW()
                    WHERE id = @Id AND isactive = true;";

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", client.Id);
                cmd.Parameters.AddWithValue("@Name", client.Name);
                cmd.Parameters.AddWithValue("@Surname", client.Surname);
                cmd.Parameters.AddWithValue("@Email", client.Email);
                cmd.Parameters.AddWithValue("@Birthdate", client.Birthdate);

                var rows = await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation("Cliente {Id} atualizado. Linhas afetadas: {Rows}", client.Id, rows);

                return rows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente {Id}", client.Id);
                throw;
            }
        }

        public async Task<bool> DeleteClientAsync(Guid id)
        {
            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                var sql = @"
                    UPDATE clients
                    SET isactive = false, updated_at = NOW()
                    WHERE id = @Id;";

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                var rows = await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation("Cliente {Id} marcado como inativo. Linhas afetadas: {Rows}", id, rows);

                return rows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir (inativar) cliente {Id}", id);
                throw;
            }
        }

    }
}
