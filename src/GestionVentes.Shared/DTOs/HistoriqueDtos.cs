using System;
using System.Collections.Generic;

namespace GestionVentes.Shared.DTOs;

public class VenteHistoriqueDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int ClientId { get; set; }
    public string ClientNom { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int NombreArticles { get; set; }
    public string? NumeroFacture { get; set; }
    public decimal TotalPaye { get; set; }
    public bool EstSoldee { get; set; }
    public List<LigneVenteDto> Lignes { get; set; } = new();
}

public class LigneVenteDto
{
    public int ProduitId { get; set; }
    public string ProduitNom { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal SousTotal => Quantite * PrixUnitaire;
}

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
}
