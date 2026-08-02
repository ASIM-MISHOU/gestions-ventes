using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionVentes.API.Data;
using GestionVentes.Shared;
using GestionVentes.Shared.DTOs;

namespace GestionVentes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentesController : ControllerBase
{
    private readonly AppDbContext _context;

    public VentesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VenteDto>>> GetVentes()
    {
        return await _context.Ventes
            .Include(v => v.Client)
            .Include(v => v.Lignes)
            .ThenInclude(l => l.Produit)
            .Select(v => new VenteDto
            {
                Id = v.Id,
                ClientId = v.ClientId,
                NomClient = v.Client != null ? v.Client.Nom : "Inconnu",
                Date = v.Date,
                Total = v.Total,
                Lignes = v.Lignes.Select(l => new LigneVenteDto
                {
                    ProduitId = l.ProduitId,
                    NomProduit = l.Produit != null ? l.Produit.Nom : "Inconnu",
                    Quantite = l.Quantite,
                    PrixUnitaire = l.PrixUnitaire
                }).ToList()
            })
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<VenteDto>> CreateVente(CreateVenteDto dto)
    {
        var client = await _context.Clients.FindAsync(dto.ClientId);
        if (client == null) return BadRequest("Client introuvable.");

        var vente = new Vente
        {
            ClientId = dto.ClientId,
            Date = DateTime.Now,
            Lignes = new List<LigneVente>()
        };

        decimal totalVente = 0;

        foreach (var item in dto.Lignes)
        {
            var produit = await _context.Produits.FindAsync(item.ProduitId);
            if (produit == null) return BadRequest($"Produit ID {item.ProduitId} introuvable.");

            if (produit.QuantiteStock < item.Quantite)
            {
                return BadRequest($"Stock insuffisant pour le produit {produit.Nom}. Stock disponible : {produit.QuantiteStock}");
            }

            // Mettre à jour le stock
            produit.QuantiteStock -= item.Quantite;

            var ligne = new LigneVente
            {
                ProduitId = produit.Id,
                Quantite = item.Quantite,
                PrixUnitaire = produit.Prix
            };

            totalVente += item.Quantite * produit.Prix;
            vente.Lignes.Add(ligne);
        }

        vente.Total = totalVente;

        // Générer automatiquement la facture liée à la vente
        vente.Facture = new Facture
        {
            Numero = $"FAC-{DateTime.Now:yyyyMMddHHmmss}",
            DateEmission = DateTime.Now
        };

        _context.Ventes.Add(vente);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVentes), new { id = vente.Id }, new VenteDto
        {
            Id = vente.Id,
            ClientId = vente.ClientId,
            NomClient = client.Nom,
            Date = vente.Date,
            Total = vente.Total
        });
    }
}