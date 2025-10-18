using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Model;
using Client.Repository;
using Client.Interfaces; // 👈 import da interface

namespace Client.Service
{
    public class ClientService : IClientService
    {
        private readonly ClientRepository _repository;

        public ClientService(ClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ClientModel>> GetAllClientsAsync(int page = 1, int pageSize = 10)
        {
            return await _repository.GetAllClientsAsync(page, pageSize);
        }

        public async Task<ClientModel?> GetClientByIdAsync(Guid id)
        {
            return await _repository.GetClientByIdAsync(id);
        }

        public async Task<Guid> AddClientAsync(ClientModel client)
        {
            return await _repository.AddClientAsync(client);
        }

        public async Task<bool> UpdateClientAsync(ClientModel client)
        {
            return await _repository.UpdateClientAsync(client);
        }

        public async Task<bool> DeleteClientAsync(Guid id)
        {
            return await _repository.DeleteClientAsync(id);
        }
    }
}
