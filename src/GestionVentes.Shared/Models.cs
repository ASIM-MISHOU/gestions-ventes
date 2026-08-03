using System;
using System.Collections.Generic;

namespace GestionVentes.Shared;

public class Client
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public ICollection<Vente> Ventes { get; set; } = new List<Vente>();
}

public class Produit
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public int QuantiteStock { get; set; }
}

public class Vente
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    public ICollection<LigneVente> Lignes { get; set; } = new List<LigneVente>();
    public Facture? Facture { get; set; }
}

public class LigneVente
{
    public int Id { get; set; }
    public int VenteId { get; set; }
    public int ProduitId { get; set; }
    public Produit? Produit { get; set; }
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
}

public class Facture
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime DateEmission { get; set; }

    // Clé étrangère et propriété de navigation vers Vente
    public int VenteId { get; set; }
    public Vente? Vente { get; set; }

    // Navigation vers les paiements
    public List<Paiement> Paiements { get; set; } = new();
}

public class Paiement
{
    public int Id { get; set; }
    public int FactureId { get; set; }
    public decimal Montant { get; set; }
    public string Mode { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
}

public class Promotion
{
    public int Id { get; set; }
    public int ProduitId { get; set; }
    public decimal Pourcentage { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
}