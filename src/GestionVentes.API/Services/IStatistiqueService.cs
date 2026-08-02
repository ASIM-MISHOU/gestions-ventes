using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionVentes.Shared.DTOs;

namespace GestionVentes.API.Services;

public interface IStatistiqueService
{
    /// <summary>Agrège toutes les statistiques nécessaires au tableau de bord commercial.</summary>
    Task<DashboardStatsDto> GetDashboardStatsAsync();

    /// <summary>Chiffre d'affaires et nombre de ventes regroupés par jour ou par mois sur une période.</summary>
    Task<List<VenteParPeriodeDto>> GetVentesParPeriodeAsync(DateTime debut, DateTime fin, string granularite);

    Task<List<TopProduitDto>> GetTopProduitsAsync(int top = 5);

    Task<List<TopClientDto>> GetTopClientsAsync(int top = 5);
}
