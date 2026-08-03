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

    // GET: api/produits
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProduitDto>>> GetProduits()
    {
        return await _context.Produits
            .Select(p => new ProduitDto
            {
                Id = p.Id,
                Nom = p.Nom,
                Prix = p.Prix,
                QuantiteStock = p.QuantiteStock
            })
            .ToListAsync();
    }

    // GET: api/produits/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProduitDto>> GetProduit(int id)
    {
        var produit = await _context.Produits.FindAsync(id);

        if (produit == null)
            return NotFound("Produit introuvable.");

        return new ProduitDto
        {
            Id = produit.Id,
            Nom = produit.Nom,
            Prix = produit.Prix,
            QuantiteStock = produit.QuantiteStock
        };
    }

    // POST: api/produits
    [HttpPost]
    public async Task<ActionResult<ProduitDto>> CreateProduit(CreateProduitDto dto)
    {
        var produit = new Produit
        {
            Nom = dto.Nom,
            Prix = dto.Prix,
            QuantiteStock = dto.QuantiteStock
        };

        _context.Produits.Add(produit);
        await _context.SaveChangesAsync();

        var result = new ProduitDto
        {
            Id = produit.Id,
            Nom = produit.Nom,
            Prix = produit.Prix,
            QuantiteStock = produit.QuantiteStock
        };

        return CreatedAtAction(nameof(GetProduit), new { id = produit.Id }, result);
    }

    // PUT: api/produits/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduit(int id, CreateProduitDto dto)
    {
        var produit = await _context.Produits.FindAsync(id);

        if (produit == null)
            return NotFound("Produit introuvable.");

        produit.Nom = dto.Nom;
        produit.Prix = dto.Prix;
        produit.QuantiteStock = dto.QuantiteStock;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/produits/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduit(int id)
    {
        var produit = await _context.Produits.FindAsync(id);

        if (produit == null)
            return NotFound("Produit introuvable.");

        _context.Produits.Remove(produit);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}