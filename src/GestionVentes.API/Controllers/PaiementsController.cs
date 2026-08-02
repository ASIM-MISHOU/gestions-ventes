using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionVentes.API.Data;
using GestionVentes.Shared;
using GestionVentes.Shared.DTOs;

namespace GestionVentes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaiementsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PaiementsController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/paiements
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaiementDto>>> GetPaiements()
    {
        var paiements = await (
            from p in _context.Paiements.AsNoTracking()
            join f in _context.Factures.AsNoTracking() on p.FactureId equals f.Id into fj
            from f in fj.DefaultIfEmpty()
            orderby p.Date descending
            select new PaiementDto
            {
                Id = p.Id,
                FactureId = p.FactureId,
                NumeroFacture = f != null ? f.Numero : string.Empty,
                Montant = p.Montant,
                Mode = p.Mode,
                Date = p.Date
            }
        ).ToListAsync();

        return Ok(paiements);
    }

    // GET api/paiements/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PaiementDto>> GetPaiement(int id)
    {
        var paiement = await _context.Paiements.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (paiement is null)
        {
            return NotFound();
        }

        var facture = await _context.Factures.AsNoTracking().FirstOrDefaultAsync(f => f.Id == paiement.FactureId);

        return Ok(new PaiementDto
        {
            Id = paiement.Id,
            FactureId = paiement.FactureId,
            NumeroFacture = facture?.Numero ?? string.Empty,
            Montant = paiement.Montant,
            Mode = paiement.Mode,
            Date = paiement.Date
        });
    }

    // GET api/paiements/facture/5 -> statut de paiement d'une facture (payée / partielle / impayée)
    [HttpGet("facture/{factureId:int}")]
    public async Task<ActionResult<FacturePaiementStatutDto>> GetStatutFacture(int factureId)
    {
        var vente = await _context.Ventes
            .Include(v => v.Facture)
                .ThenInclude(f => f!.Paiements)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Facture != null && v.Facture.Id == factureId);

        if (vente?.Facture is null)
        {
            return NotFound($"Facture {factureId} introuvable.");
        }

        var facture = vente.Facture;
        var totalPaye = facture.Paiements.Sum(p => p.Montant);

        return Ok(new FacturePaiementStatutDto
        {
            FactureId = facture.Id,
            NumeroFacture = facture.Numero,
            TotalFacture = vente.Total,
            TotalPaye = totalPaye,
            Solde = vente.Total - totalPaye,
            EstSoldee = totalPaye >= vente.Total,
            Paiements = facture.Paiements
                .OrderByDescending(p => p.Date)
                .Select(p => new PaiementDto
                {
                    Id = p.Id,
                    FactureId = p.FactureId,
                    NumeroFacture = facture.Numero,
                    Montant = p.Montant,
                    Mode = p.Mode,
                    Date = p.Date
                })
                .ToList()
        });
    }

    // POST api/paiements -> Enregistrer paiement
    [HttpPost]
    public async Task<ActionResult<PaiementDto>> EnregistrerPaiement(CreatePaiementDto dto)
    {
        if (dto.Montant <= 0)
        {
            return BadRequest("Le montant du paiement doit être supérieur à 0.");
        }

        if (string.IsNullOrWhiteSpace(dto.Mode))
        {
            return BadRequest("Le mode de paiement est obligatoire.");
        }

        var vente = await _context.Ventes
            .Include(v => v.Facture)
                .ThenInclude(f => f!.Paiements)
            .FirstOrDefaultAsync(v => v.Facture != null && v.Facture.Id == dto.FactureId);

        if (vente?.Facture is null)
        {
            return NotFound($"Facture {dto.FactureId} introuvable.");
        }

        var dejaPaye = vente.Facture.Paiements.Sum(p => p.Montant);
        var solde = vente.Total - dejaPaye;

        if (dto.Montant > solde)
        {
            return BadRequest($"Le paiement ({dto.Montant:0.00}) dépasse le solde restant ({solde:0.00}).");
        }

        var paiement = new Paiement
        {
            FactureId = dto.FactureId,
            Montant = dto.Montant,
            Mode = dto.Mode,
            Date = DateTime.Now
        };

        _context.Paiements.Add(paiement);
        await _context.SaveChangesAsync();

        var resultat = new PaiementDto
        {
            Id = paiement.Id,
            FactureId = paiement.FactureId,
            NumeroFacture = vente.Facture.Numero,
            Montant = paiement.Montant,
            Mode = paiement.Mode,
            Date = paiement.Date
        };

        return CreatedAtAction(nameof(GetPaiement), new { id = paiement.Id }, resultat);
    }

    // DELETE api/paiements/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> SupprimerPaiement(int id)
    {
        var paiement = await _context.Paiements.FindAsync(id);
        if (paiement is null)
        {
            return NotFound();
        }

        _context.Paiements.Remove(paiement);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
