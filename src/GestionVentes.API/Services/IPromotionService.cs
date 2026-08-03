using System;
using System.Threading.Tasks;
using GestionVentes.Shared;
using GestionVentes.Shared.DTOs;

namespace GestionVentes.API.Services;

public interface IPromotionService
{
    /// <summary>
    /// Retourne la promotion active (la plus avantageuse) pour un produit à une date donnée
    /// (aujourd'hui par défaut), ou null s'il n'y en a pas.
    /// </summary>
    Task<Promotion?> GetPromotionActiveAsync(int produitId, DateTime? date = null);

    /// <summary>
    /// Calcule le prix d'un produit après application de la meilleure promotion active.
    /// Utilisé par le module Ventes pour la fonction "Appliquer promotions".
    /// </summary>
    Task<PrixApresPromotionDto> CalculerPrixApresPromotionAsync(int produitId, DateTime? date = null);
}
