using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionVentes.API.Data;
using GestionVentes.API.Services;
using GestionVentes.Shared;
using GestionVentes.Shared.DTOs;

namespace GestionVentes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromotionsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPromotionService _promotionService;

    public PromotionsController(AppDbContext context, IPromotionService promotionService)
    {
        _context = context;
        _promotionService = promotionService;
    }

    // GET api/promotions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PromotionDto>>> GetPromotions()
    {
        var maintenant = DateTime.Now;

        var promotions = await (
            from promo in _context.Promotions.AsNoTracking()
            join produit in _context.Produits.AsNoTracking() on promo.ProduitId equals produit.Id into pj
            from produit in pj.DefaultIfEmpty()
            orderby promo.DateDebut descending
            select new PromotionDto
            {
                Id = promo.Id,
                ProduitId = promo.ProduitId,
                ProduitNom = produit != null ? produit.Nom : "Produit supprimé",
                Pourcentage = promo.Pourcentage,
                DateDebut = promo.DateDebut,
                DateFin = promo.DateFin,
                EstActive = promo.DateDebut <= maintenant && promo.DateFin >= maintenant
            }
        ).ToListAsync();

        return Ok(promotions);
    }

    // GET api/promotions/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PromotionDto>> GetPromotion(int id)
    {
        var promo = await _context.Promotions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (promo is null)
        {
            return NotFound();
        }

        var produit = await _context.Produits.AsNoTracking().FirstOrDefaultAsync(p => p.Id == promo.ProduitId);
        var maintenant = DateTime.Now;

        return Ok(new PromotionDto
        {
            Id = promo.Id,
            ProduitId = promo.ProduitId,
            ProduitNom = produit?.Nom ?? "Produit supprimé",
            Pourcentage = promo.Pourcentage,
            DateDebut = promo.DateDebut,
            DateFin = promo.DateFin,
            EstActive = promo.DateDebut <= maintenant && promo.DateFin >= maintenant
        });
    }

    // GET api/promotions/produit/5 -> promotion active pour un produit
    [HttpGet("produit/{produitId:int}")]
    public async Task<ActionResult<PromotionDto>> GetPromotionActiveProduit(int produitId)
    {
        var promo = await _promotionService.GetPromotionActiveAsync(produitId);
        if (promo is null)
        {
            return NotFound($"Aucune promotion active pour le produit {produitId}.");
        }

        var produit = await _context.Produits.AsNoTracking().FirstOrDefaultAsync(p => p.Id == produitId);

        return Ok(new PromotionDto
        {
            Id = promo.Id,
            ProduitId = promo.ProduitId,
            ProduitNom = produit?.Nom ?? string.Empty,
            Pourcentage = promo.Pourcentage,
            DateDebut = promo.DateDebut,
            DateFin = promo.DateFin,
            EstActive = true
        });
    }

    // GET api/promotions/produit/5/prix -> prix après promotion, utilisé par le module Ventes
    // pour "Appliquer promotions" lors du calcul du total d'une vente.
    [HttpGet("produit/{produitId:int}/prix")]
    public async Task<ActionResult<PrixApresPromotionDto>> GetPrixApresPromotion(int produitId)
    {
        try
        {
            var resultat = await _promotionService.CalculerPrixApresPromotionAsync(produitId);
            return Ok(resultat);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // POST api/promotions -> Ajouter promotion
    [HttpPost]
    public async Task<ActionResult<PromotionDto>> CreerPromotion(CreatePromotionDto dto)
    {
        if (dto.Pourcentage <= 0 || dto.Pourcentage > 100)
        {
            return BadRequest("Le pourcentage doit être compris entre 0 et 100.");
        }

        if (dto.DateFin < dto.DateDebut)
        {
            return BadRequest("La date de fin doit être postérieure ou égale à la date de début.");
        }

        var produit = await _context.Produits.FindAsync(dto.ProduitId);
        if (produit is null)
        {
            return NotFound($"Produit {dto.ProduitId} introuvable.");
        }

        var promotion = new Promotion
        {
            ProduitId = dto.ProduitId,
            Pourcentage = dto.Pourcentage,
            DateDebut = dto.DateDebut,
            DateFin = dto.DateFin
        };

        _context.Promotions.Add(promotion);
        await _context.SaveChangesAsync();

        var maintenant = DateTime.Now;
        return CreatedAtAction(nameof(GetPromotion), new { id = promotion.Id }, new PromotionDto
        {
            Id = promotion.Id,
            ProduitId = promotion.ProduitId,
            ProduitNom = produit.Nom,
            Pourcentage = promotion.Pourcentage,
            DateDebut = promotion.DateDebut,
            DateFin = promotion.DateFin,
            EstActive = promotion.DateDebut <= maintenant && promotion.DateFin >= maintenant
        });
    }

    // PUT api/promotions/5 -> Modifier promotion
    [HttpPut("{id:int}")]
    public async Task<IActionResult> ModifierPromotion(int id, UpdatePromotionDto dto)
    {
        var promotion = await _context.Promotions.FindAsync(id);
        if (promotion is null)
        {
            return NotFound();
        }

        if (dto.Pourcentage <= 0 || dto.Pourcentage > 100)
        {
            return BadRequest("Le pourcentage doit être compris entre 0 et 100.");
        }

        if (dto.DateFin < dto.DateDebut)
        {
            return BadRequest("La date de fin doit être postérieure ou égale à la date de début.");
        }

        promotion.Pourcentage = dto.Pourcentage;
        promotion.DateDebut = dto.DateDebut;
        promotion.DateFin = dto.DateFin;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE api/promotions/5 -> Supprimer promotion
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> SupprimerPromotion(int id)
    {
        var promotion = await _context.Promotions.FindAsync(id);
        if (promotion is null)
        {
            return NotFound();
        }

        _context.Promotions.Remove(promotion);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
