using System;
using System.Collections.Generic;

namespace GestionVentes.Shared.DTOs;

public class FactureDto
{
    public int Id { get; set; }
    public int VenteId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime DateEmission { get; set; }
    public decimal MontantTotal { get; set; }
    public decimal ResteAPayer { get; set; }
}