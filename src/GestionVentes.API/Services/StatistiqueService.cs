using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionVentes.API.Data;
using GestionVentes.Shared;
using GestionVentes.Shared.DTOs;

namespace GestionVentes.API.Services;

public class StatistiqueService : IStatistiqueService
{
    private readonly AppDbContext _context;

    public StatistiqueService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var maintenant = DateTime.Now;
        var debutJour = maintenant.Date;
        var debutMois = new DateTime(maintenant.Year, maintenant.Month, 1);

        // On charge les ventes avec leurs relations en mémoire : le volume attendu pour un
        // projet scolaire reste faible, et cela évite les limitations de traduction LINQ -> SQL
        // de SQLite pour les regroupements par date.
        var ventes = await _context.Ventes
            .Include(v => v.Client)
            .Include(v => v.Lignes)
                .ThenInclude(l => l.Produit)
            .Include(v => v.Facture)
                .ThenInclude(f => f!.Paiements)
            .AsNoTracking()
            .ToListAsync();

        var dashboard = new DashboardStatsDto
        {
            ChiffreAffairesTotal = ventes.Sum(v => v.Total),
            ChiffreAffairesJour = ventes.Where(v => v.Date >= debutJour).Sum(v => v.Total),
            ChiffreAffairesMois = ventes.Where(v => v.Date >= debutMois).Sum(v => v.Total),
            NombreVentesTotal = ventes.Count,
            NombreVentesJour = ventes.Count(v => v.Date >= debutJour),
            NombreVentesMois = ventes.Count(v => v.Date >= debutMois),
            NombreClients = await _context.Clients.CountAsync(),
            NombreProduits = await _context.Produits.CountAsync(),
            PanierMoyen = ventes.Count > 0 ? Math.Round(ventes.Average(v => v.Total), 2) : 0
        };

        dashboard.TopProduits = ventes
            .SelectMany(v => v.Lignes)
            .GroupBy(l => new { l.ProduitId, Nom = l.Produit?.Nom ?? "Produit supprimé" })
            .Select(g => new TopProduitDto
            {
                ProduitId = g.Key.ProduitId,
                Nom = g.Key.Nom,
                QuantiteVendue = g.Sum(l => l.Quantite),
                ChiffreAffaires = g.Sum(l => l.Quantite * l.PrixUnitaire)
            })
            .OrderByDescending(p => p.ChiffreAffaires)
            .Take(5)
            .ToList();

        dashboard.TopClients = ventes
            .GroupBy(v => new { v.ClientId, Nom = v.Client?.Nom ?? "Client supprimé" })
            .Select(g => new TopClientDto
            {
                ClientId = g.Key.ClientId,
                Nom = g.Key.Nom,
                NombreAchats = g.Count(),
                TotalDepense = g.Sum(v => v.Total)
            })
            .OrderByDescending(c => c.TotalDepense)
            .Take(5)
            .ToList();

        dashboard.VentesParJour = ventes
            .Where(v => v.Date >= maintenant.AddDays(-6).Date)
            .GroupBy(v => v.Date.Date)
            .Select(g => new VenteParPeriodeDto
            {
                Periode = g.Key.ToString("yyyy-MM-dd"),
                ChiffreAffaires = g.Sum(v => v.Total),
                NombreVentes = g.Count()
            })
            .OrderBy(v => v.Periode)
            .ToList();

        dashboard.VentesParMois = ventes
            .Where(v => v.Date >= maintenant.AddMonths(-5))
            .GroupBy(v => new DateTime(v.Date.Year, v.Date.Month, 1))
            .Select(g => new VenteParPeriodeDto
            {
                Periode = g.Key.ToString("yyyy-MM"),
                ChiffreAffaires = g.Sum(v => v.Total),
                NombreVentes = g.Count()
            })
            .OrderBy(v => v.Periode)
            .ToList();

        dashboard.StatutsPaiement = ventes
            .Where(v => v.Facture is not null)
            .GroupBy(v =>
            {
                var totalPaye = v.Facture!.Paiements.Sum(p => p.Montant);
                if (totalPaye <= 0) return "Impayée";
                return totalPaye < v.Total ? "Partielle" : "Payée";
            })
            .Select(g => new StatutPaiementDto
            {
                Statut = g.Key,
                NombreFactures = g.Count(),
                Montant = g.Sum(v => v.Total)
            })
            .ToList();

        return dashboard;
    }

    public async Task<List<VenteParPeriodeDto>> GetVentesParPeriodeAsync(DateTime debut, DateTime fin, string granularite)
    {
        var ventes = await _context.Ventes
            .AsNoTracking()
            .Where(v => v.Date >= debut && v.Date <= fin)
            .ToListAsync();

        IEnumerable<IGrouping<string, Vente>> groupes = string.Equals(granularite, "mois", StringComparison.OrdinalIgnoreCase)
            ? ventes.GroupBy(v => v.Date.ToString("yyyy-MM"))
            : ventes.GroupBy(v => v.Date.ToString("yyyy-MM-dd"));

        return groupes
            .Select(g => new VenteParPeriodeDto
            {
                Periode = g.Key,
                ChiffreAffaires = g.Sum(v => v.Total),
                NombreVentes = g.Count()
            })
            .OrderBy(v => v.Periode)
            .ToList();
    }

    public async Task<List<TopProduitDto>> GetTopProduitsAsync(int top = 5)
    {
        var lignes = await _context.LignesVente
            .Include(l => l.Produit)
            .AsNoTracking()
            .ToListAsync();

        return lignes
            .GroupBy(l => new { l.ProduitId, Nom = l.Produit?.Nom ?? "Produit supprimé" })
            .Select(g => new TopProduitDto
            {
                ProduitId = g.Key.ProduitId,
                Nom = g.Key.Nom,
                QuantiteVendue = g.Sum(l => l.Quantite),
                ChiffreAffaires = g.Sum(l => l.Quantite * l.PrixUnitaire)
            })
            .OrderByDescending(p => p.ChiffreAffaires)
            .Take(top)
            .ToList();
    }

    public async Task<List<TopClientDto>> GetTopClientsAsync(int top = 5)
    {
        var ventes = await _context.Ventes
            .Include(v => v.Client)
            .AsNoTracking()
            .ToListAsync();

        return ventes
            .GroupBy(v => new { v.ClientId, Nom = v.Client?.Nom ?? "Client supprimé" })
            .Select(g => new TopClientDto
            {
                ClientId = g.Key.ClientId,
                Nom = g.Key.Nom,
                NombreAchats = g.Count(),
                TotalDepense = g.Sum(v => v.Total)
            })
            .OrderByDescending(c => c.TotalDepense)
            .Take(top)
            .ToList();
    }
}
