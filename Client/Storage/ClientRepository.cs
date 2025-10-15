using Client.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Repository
{
    public class ClientRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ClientRepository> _logger;

        public ClientRepository(IConfiguration configuration, ILogger<ClientRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<List<ClientModel>> GetAllClientsAsync(int page = 1, int pageSize = 10)
        {
            var clients = new List<ClientModel>();
            var offset = (page - 1) * pageSize;

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"SELECT id, name, email, isactive, created_at, updated_at
                        FROM clients
                        WHERE isactive = true
                        ORDER BY created_at
                        LIMIT @PageSize OFFSET @Offset;";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);
            cmd.Parameters.AddWithValue("@Offset", offset);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                clients.Add(new ClientModel
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    IsActive = reader.GetBoolean(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.GetDateTime(5)
                });
            }

            _logger.LogInformation("Retornados {Count} clientes (page {Page}, size {PageSize})",
                clients.Count, page, pageSize);

            return clients;
        }

        public async Task<ClientModel?> GetClientByIdAsync(Guid id)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"SELECT id, name, email, isactive, created_at, updated_at
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
                    Email = reader.GetString(2),
                    IsActive = reader.GetBoolean(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.GetDateTime(5)
                };
            }

            return null;
        }

        public async Task<Guid> AddClientAsync(ClientModel client)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"INSERT INTO clients (name, email, isactive, created_at, updated_at)
                        VALUES (@Name, @Email, @IsActive, NOW(), NOW())
                        RETURNING id;";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", client.Name);
            cmd.Parameters.AddWithValue("@Email", client.Email);
            cmd.Parameters.AddWithValue("@IsActive", client.IsActive);

            var id = (Guid)await cmd.ExecuteScalarAsync();
            _logger.LogInformation("Cliente {Name} criado com Id {Id}", client.Name, id);

            return id;
        }

        public async Task<bool> UpdateClientAsync(ClientModel client)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"UPDATE clients
                        SET name = @Name, email = @Email, updated_at = NOW()
                        WHERE id = @Id AND isactive = true;";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", client.Id);
            cmd.Parameters.AddWithValue("@Name", client.Name);
            cmd.Parameters.AddWithValue("@Email", client.Email);

            var rows = await cmd.ExecuteNonQueryAsync();
            _logger.LogInformation("Cliente {Id} atualizado. Rows afetadas: {Rows}", client.Id, rows);

            return rows > 0;
        }

        public async Task<bool> DeleteClientAsync(Guid id)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var sql = @"UPDATE clients
                        SET isactive = false, updated_at = NOW()
                        WHERE id = @Id;";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            var rows = await cmd.ExecuteNonQueryAsync();
            _logger.LogInformation("Cliente {Id} marcado como inativo. Rows afetadas: {Rows}", id, rows);

            return rows > 0;
        }
    }
}
