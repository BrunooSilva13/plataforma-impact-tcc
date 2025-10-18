using Microsoft.AspNetCore.Mvc;
using Client.Model;
using Client.Service;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ClientService _service;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(ClientService service, ILogger<ClientsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/clients?page=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientModel>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Buscando clientes. Página {Page}, Tamanho {PageSize}", page, pageSize);

            var clients = await _service.GetAllClientsAsync(page, pageSize);

            _logger.LogInformation("Retornados {Count} clientes", clients.Count);

            return Ok(clients);
        }

        // GET: api/clients/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ClientModel>> GetById(Guid id)
        {
            _logger.LogInformation("Buscando cliente com ID {ClientId}", id);

            var client = await _service.GetClientByIdAsync(id);

            if (client == null)
            {
                _logger.LogWarning("Cliente com ID {ClientId} não encontrado", id);
                return NotFound(new { message = $"Cliente com ID {id} não encontrado." });
            }

            return Ok(client);
        }

        // POST: api/clients
        [HttpPost]
        public async Task<ActionResult<ClientModel>> Create([FromBody] ClientModel client)
        {
            if (client == null)
            {
                _logger.LogWarning("Tentativa de criar cliente sem corpo na requisição");
                return BadRequest(new { message = "Cliente não informado." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validação falhou para cliente: {@Client}", client);
                return BadRequest(ModelState);
            }

            var id = await _service.AddClientAsync(client);
            client.Id = id;

            _logger.LogInformation("Cliente criado com sucesso. Id: {ClientId}", id);

            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        // PUT: api/clients/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ClientModel client)
        {
            if (client == null)
            {
                _logger.LogWarning("Tentativa de atualizar cliente sem corpo na requisição");
                return BadRequest(new { message = "Cliente não informado." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validação falhou para cliente: {@Client}", client);
                return BadRequest(ModelState);
            }

            if (id != client.Id)
            {
                _logger.LogWarning("O ID da URL {UrlId} não corresponde ao ID enviado {ClientId}", id, client.Id);
                return BadRequest(new { message = "O ID da URL não corresponde ao ID do cliente enviado." });
            }

            var updated = await _service.UpdateClientAsync(client);

            if (!updated)
            {
                _logger.LogWarning("Cliente com ID {ClientId} não encontrado ou inativo", id);
                return NotFound(new { message = $"Cliente com ID {id} não encontrado ou inativo." });
            }

            _logger.LogInformation("Cliente com ID {ClientId} atualizado com sucesso", id);

            return NoContent();
        }

        // DELETE: api/clients/{id} (soft delete)
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Solicitação de exclusão do cliente com ID {ClientId}", id);

            var deleted = await _service.DeleteClientAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Cliente com ID {ClientId} não encontrado", id);
                return NotFound(new { message = $"Cliente com ID {id} não encontrado." });
            }

            _logger.LogInformation("Cliente com ID {ClientId} marcado como inativo (soft delete)", id);

            return NoContent();
        }
    }
}
