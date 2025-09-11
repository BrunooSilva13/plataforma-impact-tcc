using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Client.Domain;

namespace Client.Storage
{
    public class ClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddClientAsync(ClientModel client)
        {
            var query = @"
                INSERT INTO clients (id, name, surname, email, birthdate, created_at, updated_at, isactive)
                VALUES (@Id, @Name, @Surname, @Email, @Birthdate, @CreatedAt, @UpdatedAt, @IsActive);
            ";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("Id", client.Id);
            cmd.Parameters.AddWithValue("Name", client.Name);
            cmd.Parameters.AddWithValue("Surname", client.Surname);
            cmd.Parameters.AddWithValue("Email", client.Email);
            cmd.Parameters.AddWithValue("Birthdate", client.Birthdate);
            cmd.Parameters.AddWithValue("CreatedAt", client.CreatedAt);
            cmd.Parameters.AddWithValue("UpdatedAt", client.UpdatedAt);
            cmd.Parameters.AddWithValue("IsActive", client.IsActive);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<ClientModel>> GetAllClientsAsync()
        {
            var clients = new List<ClientModel>();
            var query = "SELECT * FROM clients WHERE isactive = true;";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                clients.Add(new ClientModel
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Surname = reader.GetString(reader.GetOrdinal("surname")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    Birthdate = reader.GetDateTime(reader.GetOrdinal("birthdate")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("isactive"))
                });
            }

            return clients;
        }

        public async Task<ClientModel?> GetClientByIdAsync(Guid clientId)
        {
            var query = "SELECT * FROM clients WHERE id = @Id AND isactive = true;";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("Id", clientId);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ClientModel
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Surname = reader.GetString(reader.GetOrdinal("surname")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    Birthdate = reader.GetDateTime(reader.GetOrdinal("birthdate")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("isactive"))
                };
            }

            return null;
        }

        public async Task UpdateClientAsync(ClientModel client)
        {
            var query = @"
                UPDATE clients
                SET name = @Name,
                    surname = @Surname,
                    email = @Email,
                    birthdate = @Birthdate,
                    updated_at = @UpdatedAt
                WHERE id = @Id;
            ";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("Id", client.Id);
            cmd.Parameters.AddWithValue("Name", client.Name);
            cmd.Parameters.AddWithValue("Surname", client.Surname);
            cmd.Parameters.AddWithValue("Email", client.Email);
            cmd.Parameters.AddWithValue("Birthdate", client.Birthdate);
            cmd.Parameters.AddWithValue("UpdatedAt", DateTime.UtcNow);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteClientAsync(Guid clientId)
        {
            var query = "UPDATE clients SET isactive = false, updated_at = @UpdatedAt WHERE id = @Id;";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("Id", clientId);
            cmd.Parameters.AddWithValue("UpdatedAt", DateTime.UtcNow);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
