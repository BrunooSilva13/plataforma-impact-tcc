using Microsoft.AspNetCore.Mvc;
using Client.Domain;
using Client.Storage;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Client.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ClientRepository _repository;

        public ClientController(ClientRepository repository)
        {
            _repository = repository;
        }

        // GET: api/client
        [HttpGet]
        public async Task<ActionResult<List<ClientModel>>> GetAll()
        {
            var clients = await _repository.GetAllClientsAsync();
            return Ok(clients);
        }

        // GET: api/client/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientModel>> GetById(Guid id)
        {
            var client = await _repository.GetClientByIdAsync(id);
            if (client == null)
                return NotFound();
            return Ok(client);
        }

        // POST: api/client
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ClientModel client)
        {
            await _repository.AddClientAsync(client);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        // PUT: api/client/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] ClientModel client)
        {
            var existingClient = await _repository.GetClientByIdAsync(id);
            if (existingClient == null)
                return NotFound();

            client.Id = id; // Garantir que o ID não seja alterado
            await _repository.UpdateClientAsync(client);
            return NoContent();
        }

        // DELETE: api/client/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var existingClient = await _repository.GetClientByIdAsync(id);
            if (existingClient == null)
                return NotFound();

            await _repository.DeleteClientAsync(id);
            return NoContent();
        }
    }

