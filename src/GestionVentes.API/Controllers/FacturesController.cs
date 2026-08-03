using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionVentes.API.Data;
using GestionVentes.Shared;
using GestionVentes.Shared.DTOs;

namespace GestionVentes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacturesController : ControllerBase
{
    private readonly AppDbContext _context;

    public FacturesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/factures
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FactureDto>>> GetFactures()
    {
        return await _context.Factures
            .Include(f => f.Vente)
            .Include(f => f.Paiements)
            .Select(f => new FactureDto
            {
                Id = f.Id,
                VenteId = f.VenteId,
                Numero = f.Numero,
                DateEmission = f.DateEmission,
                MontantTotal = f.Vente != null ? f.Vente.Total : 0,
                ResteAPayer = (f.Vente != null ? f.Vente.Total : 0) - f.Paiements.Sum(p => p.Montant)
            })
            .ToListAsync();
    }

    // GET: api/factures/5
    [HttpGet("{id}")]
    public async Task<ActionResult<FactureDto>> GetFacture(int id)
    {
        var f = await _context.Factures
            .Include(f => f.Vente)
            .Include(f => f.Paiements)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (f == null) return NotFound("Facture non trouvée.");

        var total = f.Vente?.Total ?? 0;
        var totalPaye = f.Paiements.Sum(p => p.Montant);

        return new FactureDto
        {
            Id = f.Id,
            VenteId = f.VenteId,
            Numero = f.Numero,
            DateEmission = f.DateEmission,
            MontantTotal = total,
            ResteAPayer = total - totalPaye
        };
    }

    // POST: api/factures/5/paiements
    [HttpPost("{id}/paiements")]
    public async Task<ActionResult<PaiementDto>> AjouterPaiement(int id, CreatePaiementDto dto)
    {
        var facture = await _context.Factures
            .Include(f => f.Vente)
            .Include(f => f.Paiements)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (facture == null) return NotFound("Facture non trouvée.");

        var totalVente = facture.Vente?.Total ?? 0;
        var dejaPaye = facture.Paiements.Sum(p => p.Montant);
        var resteAPayer = totalVente - dejaPaye;

        if (dto.Montant <= 0)
        {
            return BadRequest("Le montant du paiement doit être supérieur à zéro.");
        }

        if (dto.Montant > resteAPayer)
        {
            return BadRequest($"Le montant dépasse le reste à payer. Reste dû : {resteAPayer:C}");
        }

        var paiement = new Paiement
        {
            FactureId = id,
            Montant = dto.Montant,
            Mode = dto.Mode,
            Date = DateTime.Now
        };

        _context.Paiements.Add(paiement);
        await _context.SaveChangesAsync();

        return Ok(new PaiementDto
        {
            Id = paiement.Id,
            FactureId = paiement.FactureId,
            Montant = paiement.Montant,
            Mode = paiement.Mode,
            Date = paiement.Date
        });
    }
}