using System;
using System.Collections.Generic;

namespace GestionVentes.Shared.DTOs;

public class LigneVenteDto
{
    public int ProduitId { get; set; }
    public string NomProduit { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal SousTotal => Quantite * PrixUnitaire;
}

public class VenteDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string NomClient { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    public List<LigneVenteDto> Lignes { get; set; } = new();
}

public class CreateLigneVenteDto
{
    public int ProduitId { get; set; }
    public int Quantite { get; set; }
}

public class CreateVenteDto
{
    public int ClientId { get; set; }
    public List<CreateLigneVenteDto> Lignes { get; set; } = new();
}