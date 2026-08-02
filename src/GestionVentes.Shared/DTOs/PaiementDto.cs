using System;

namespace GestionVentes.Shared.DTOs;

public class PaiementDto
{
    public int Id { get; set; }
    public int FactureId { get; set; }
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