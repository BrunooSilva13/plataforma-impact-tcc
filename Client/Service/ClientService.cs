using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Domain;
using Client.Storage;

namespace Client.Service
{
    public class ClientService
    {
        private readonly ClientRepository _repository;

        public ClientService(ClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ClientModel>> GetAllClientsAsync()
        {
            return await _repository.GetAllClientsAsync();
        }

        public async Task<ClientModel?> GetClientByIdAsync(Guid id)
        {
            return await _repository.GetClientByIdAsync(id);
        }

        public async Task AddClientAsync(ClientModel client)
        {
            await _repository.AddClientAsync(client);
        }

        public async Task UpdateClientAsync(ClientModel client)
        {
            await _repository.UpdateClientAsync(client);
        }

        public async Task DeleteClientAsync(Guid id)
        {
            await _repository.DeleteClientAsync(id);
        }
    }
}
