using System;
using System.Collections.Generic;

namespace GestionVentes.Shared.DTOs;

public class PaiementDto
{
    public int Id { get; set; }
    public int FactureId { get; set; }
    public string NumeroFacture { get; set; } = string.Empty;
    public decimal Montant { get; set; }
    public string Mode { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public class CreatePaiementDto
{
    public int FactureId { get; set; }
    public decimal Montant { get; set; }
    public string Mode { get; set; } = string.Empty;
}

public class FacturePaiementStatutDto
{
    public int FactureId { get; set; }
    public string NumeroFacture { get; set; } = string.Empty;
    public decimal TotalFacture { get; set; }
    public decimal TotalPaye { get; set; }
    public decimal Solde { get; set; }
    public bool EstSoldee { get; set; }
    public List<PaiementDto> Paiements { get; set; } = new();
}
