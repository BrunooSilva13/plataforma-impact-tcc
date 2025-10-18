using Client.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<ClientModel>> GetAllClientsAsync(int page, int pageSize);
        Task<ClientModel?> GetClientByIdAsync(Guid id);
        Task<Guid> AddClientAsync(ClientModel client);
        Task<bool> UpdateClientAsync(ClientModel client);
        Task<bool> DeleteClientAsync(Guid id);
    }
}
