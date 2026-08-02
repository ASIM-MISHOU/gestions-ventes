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
public class HistoriqueVentesController : ControllerBase
{
    private readonly AppDbContext _context;

    public HistoriqueVentesController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/historiqueventes?dateDebut=&dateFin=&clientId=&produitId=&page=1&pageSize=20
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<VenteHistoriqueDto>>> GetHistorique(
        [FromQuery] DateTime? dateDebut,
        [FromQuery] DateTime? dateFin,
        [FromQuery] int? clientId,
        [FromQuery] int? produitId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 200) pageSize = 20;

        var query = _context.Ventes
            .Include(v => v.Client)
            .Include(v => v.Facture)
                .ThenInclude(f => f!.Paiements)
            .Include(v => v.Lignes)
                .ThenInclude(l => l.Produit)
            .AsNoTracking()
            .AsQueryable();

        if (dateDebut.HasValue)
            query = query.Where(v => v.Date >= dateDebut.Value);

        if (dateFin.HasValue)
            query = query.Where(v => v.Date <= dateFin.Value);

        if (clientId.HasValue)
            query = query.Where(v => v.ClientId == clientId.Value);

        if (produitId.HasValue)
            query = query.Where(v => v.Lignes.Any(l => l.ProduitId == produitId.Value));

        query = query.OrderByDescending(v => v.Date);

        var totalCount = await query.CountAsync();

        var ventes = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new PagedResultDto<VenteHistoriqueDto>
        {
            Items = ventes.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    // GET api/historiqueventes/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VenteHistoriqueDto>> GetVente(int id)
    {
        var vente = await _context.Ventes
            .Include(v => v.Client)
            .Include(v => v.Facture)
                .ThenInclude(f => f!.Paiements)
            .Include(v => v.Lignes)
                .ThenInclude(l => l.Produit)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vente is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(vente));
    }

    // GET api/historiqueventes/client/5
    [HttpGet("client/{clientId:int}")]
    public async Task<ActionResult<IEnumerable<VenteHistoriqueDto>>> GetHistoriqueClient(int clientId)
    {
        var ventes = await _context.Ventes
            .Include(v => v.Client)
            .Include(v => v.Facture)
                .ThenInclude(f => f!.Paiements)
            .Include(v => v.Lignes)
                .ThenInclude(l => l.Produit)
            .AsNoTracking()
            .Where(v => v.ClientId == clientId)
            .OrderByDescending(v => v.Date)
            .ToListAsync();

        return Ok(ventes.Select(MapToDto).ToList());
    }

    private static VenteHistoriqueDto MapToDto(Vente v)
    {
        var totalPaye = v.Facture?.Paiements.Sum(p => p.Montant) ?? 0m;

        return new VenteHistoriqueDto
        {
            Id = v.Id,
            Date = v.Date,
            ClientId = v.ClientId,
            ClientNom = v.Client?.Nom ?? "Client supprimé",
            Total = v.Total,
            NombreArticles = v.Lignes.Sum(l => l.Quantite),
            NumeroFacture = v.Facture?.Numero,
            TotalPaye = totalPaye,
            EstSoldee = v.Facture != null && totalPaye >= v.Total,
            Lignes = v.Lignes.Select(l => new LigneVenteDto
            {
                ProduitId = l.ProduitId,
                ProduitNom = l.Produit?.Nom ?? "Produit supprimé",
                Quantite = l.Quantite,
                PrixUnitaire = l.PrixUnitaire
            }).ToList()
        };
    }
}
