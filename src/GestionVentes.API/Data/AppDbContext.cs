using Microsoft.EntityFrameworkCore;
using GestionVentes.Shared;

namespace GestionVentes.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Produit> Produits => Set<Produit>();
    public DbSet<Vente> Ventes => Set<Vente>();
    public DbSet<LigneVente> LignesVente => Set<LigneVente>();
    public DbSet<Facture> Factures => Set<Facture>();
    public DbSet<Paiement> Paiements => Set<Paiement>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
}