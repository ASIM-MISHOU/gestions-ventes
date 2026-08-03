using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionVentes.API.Data;
using GestionVentes.Shared;
using GestionVentes.Shared.DTOs;

namespace GestionVentes.API.Services;

public class PromotionService : IPromotionService
{
    private readonly AppDbContext _context;

    public PromotionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Promotion?> GetPromotionActiveAsync(int produitId, DateTime? date = null)
    {
        var reference = date ?? DateTime.Now;

        return await _context.Promotions
            .Where(p => p.ProduitId == produitId
                        && p.DateDebut <= reference
                        && p.DateFin >= reference)
            .OrderByDescending(p => p.Pourcentage)
            .FirstOrDefaultAsync();
    }

    public async Task<PrixApresPromotionDto> CalculerPrixApresPromotionAsync(int produitId, DateTime? date = null)
    {
        var produit = await _context.Produits.FindAsync(produitId);
        if (produit is null)
        {
            throw new KeyNotFoundException($"Produit {produitId} introuvable.");
        }

        var promotion = await GetPromotionActiveAsync(produitId, date);

        var resultat = new PrixApresPromotionDto
        {
            ProduitId = produitId,
            PrixOriginal = produit.Prix,
            PromotionAppliquee = promotion is not null
        };

        if (promotion is not null)
        {
            var reduction = produit.Prix * (promotion.Pourcentage / 100m);
            resultat.PrixFinal = Math.Round(produit.Prix - reduction, 2);
            resultat.PourcentageApplique = promotion.Pourcentage;
        }
        else
        {
            resultat.PrixFinal = produit.Prix;
        }

        return resultat;
    }
}
