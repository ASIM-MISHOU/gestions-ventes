using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionVentes.API.Data;
using GestionVentes.Shared;
using GestionVentes.Shared.DTOs;

namespace GestionVentes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProduitsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProduitsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProduitDto>>> GetProduits()
    {
        return await _context.Produits
            .Select(p => new ProduitDto {
                Id = p.Id,
                Nom = p.Nom,
                Prix = p.Prix,
                QuantiteStock = p.QuantiteStock
            })
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<ProduitDto>> CreateProduit(CreateProduitDto dto)
    {
        var produit = new Produit {
            Nom = dto.Nom,
            Prix = dto.Prix,
            QuantiteStock = dto.QuantiteStock
        };

        _context.Produits.Add(produit);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduits), new { id = produit.Id }, new ProduitDto {
            Id = produit.Id,
            Nom = produit.Nom,
            Prix = produit.Prix,
            QuantiteStock = produit.QuantiteStock
        });
    }
}