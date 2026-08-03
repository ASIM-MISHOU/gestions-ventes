using System;

namespace GestionVentes.Shared.DTOs;

public class PromotionDto
{
    public int Id { get; set; }
    public int ProduitId { get; set; }
    public string ProduitNom { get; set; } = string.Empty;
    public decimal Pourcentage { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public bool EstActive { get; set; }
}

public class CreatePromotionDto
{
    public int ProduitId { get; set; }
    public decimal Pourcentage { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
}

public class UpdatePromotionDto
{
    public decimal Pourcentage { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
}

public class PrixApresPromotionDto
{
    public int ProduitId { get; set; }
    public decimal PrixOriginal { get; set; }
    public decimal PrixFinal { get; set; }
    public decimal? PourcentageApplique { get; set; }
    public bool PromotionAppliquee { get; set; }
}
