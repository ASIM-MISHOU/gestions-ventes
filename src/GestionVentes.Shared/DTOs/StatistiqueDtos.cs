using System.Collections.Generic;

namespace GestionVentes.Shared.DTOs;

public class DashboardStatsDto
{
    public decimal ChiffreAffairesTotal { get; set; }
    public decimal ChiffreAffairesJour { get; set; }
    public decimal ChiffreAffairesMois { get; set; }
    public int NombreVentesTotal { get; set; }
    public int NombreVentesJour { get; set; }
    public int NombreVentesMois { get; set; }
    public int NombreClients { get; set; }
    public int NombreProduits { get; set; }
    public decimal PanierMoyen { get; set; }
    public List<TopProduitDto> TopProduits { get; set; } = new();
    public List<TopClientDto> TopClients { get; set; } = new();
    public List<VenteParPeriodeDto> VentesParJour { get; set; } = new();
    public List<VenteParPeriodeDto> VentesParMois { get; set; } = new();
    public List<StatutPaiementDto> StatutsPaiement { get; set; } = new();
}

public class TopProduitDto
{
    public int ProduitId { get; set; }
    public string Nom { get; set; } = string.Empty;
    public int QuantiteVendue { get; set; }
    public decimal ChiffreAffaires { get; set; }
}

public class TopClientDto
{
    public int ClientId { get; set; }
    public string Nom { get; set; } = string.Empty;
    public int NombreAchats { get; set; }
    public decimal TotalDepense { get; set; }
}

public class VenteParPeriodeDto
{
    /// <summary>Format "yyyy-MM-dd" pour la granularité jour, "yyyy-MM" pour mois.</summary>
    public string Periode { get; set; } = string.Empty;
    public decimal ChiffreAffaires { get; set; }
    public int NombreVentes { get; set; }
}

public class StatutPaiementDto
{
    /// <summary>"Payée", "Partielle" ou "Impayée".</summary>
    public string Statut { get; set; } = string.Empty;
    public int NombreFactures { get; set; }
    public decimal Montant { get; set; }
}
