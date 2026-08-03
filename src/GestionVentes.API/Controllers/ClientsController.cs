using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionVentes.API.Data;
using GestionVentes.Shared;
using GestionVentes.Shared.DTOs;

namespace GestionVentes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/clients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
    {
        return await _context.Clients
            .Select(c => new ClientDto
            {
                Id = c.Id,
                Nom = c.Nom,
                Email = c.Email,
                Telephone = c.Telephone
            })
            .ToListAsync();
    }

    // GET: api/clients/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetClient(int id)
    {
        var client = await _context.Clients.FindAsync(id);

        if (client == null)
            return NotFound("Client introuvable.");

        return new ClientDto
        {
            Id = client.Id,
            Nom = client.Nom,
            Email = client.Email,
            Telephone = client.Telephone
        };
    }

    // POST: api/clients
    [HttpPost]
    public async Task<ActionResult<ClientDto>> CreateClient(CreateClientDto dto)
    {
        var client = new Client
        {
            Nom = dto.Nom,
            Email = dto.Email,
            Telephone = dto.Telephone
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var result = new ClientDto
        {
            Id = client.Id,
            Nom = client.Nom,
            Email = client.Email,
            Telephone = client.Telephone
        };

        return CreatedAtAction(nameof(GetClient), new { id = client.Id }, result);
    }

    // PUT: api/clients/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClient(int id, CreateClientDto dto)
    {
        var client = await _context.Clients.FindAsync(id);

        if (client == null)
            return NotFound("Client introuvable.");

        client.Nom = dto.Nom;
        client.Email = dto.Email;
        client.Telephone = dto.Telephone;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/clients/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var client = await _context.Clients.FindAsync(id);

        if (client == null)
            return NotFound("Client introuvable.");

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}