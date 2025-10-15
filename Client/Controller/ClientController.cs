using Microsoft.AspNetCore.Mvc;
using Client.Model;
using Client.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ClientService _service;

        public ClientController(ClientService service)
        {
            _service = service;
        }

        // GET: api/client?page=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientModel>>> GetAllClients([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var clients = await _service.GetAllClientsAsync(page, pageSize);
            return Ok(clients);
        }

        // GET: api/client/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ClientModel>> GetClientById(Guid id)
        {
            var client = await _service.GetClientByIdAsync(id);

            if (client == null)
                return NotFound(new { message = $"Cliente com ID {id} não encontrado." });

            return Ok(client);
        }

        // POST: api/client
        [HttpPost]
        public async Task<ActionResult<ClientModel>> AddClient([FromBody] ClientModel client)
        {
            // Validação do modelo
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _service.AddClientAsync(client);
            client.Id = id;

            return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, client);
        }

        // PUT: api/client/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateClient(Guid id, [FromBody] ClientModel client)
        {
            // Validação do modelo
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != client.Id)
                return BadRequest(new { message = "O ID da URL não corresponde ao ID do cliente enviado." });

            var updated = await _service.UpdateClientAsync(client);

            if (!updated)
                return NotFound(new { message = $"Cliente com ID {id} não encontrado ou inativo." });

            return NoContent();
        }

        // DELETE: api/client/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            var deleted = await _service.DeleteClientAsync(id);

            if (!deleted)
                return NotFound(new { message = $"Cliente com ID {id} não encontrado." });

            return NoContent();
        }
    }
}
