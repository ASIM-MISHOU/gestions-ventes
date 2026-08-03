using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GestionVentes.API.Services;
using GestionVentes.Shared.DTOs;

namespace GestionVentes.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatistiquesController : ControllerBase
{
    private readonly IStatistiqueService _statistiqueService;

    public StatistiquesController(IStatistiqueService statistiqueService)
    {
        _statistiqueService = statistiqueService;
    }

    // GET api/statistiques/dashboard -> alimente le tableau de bord commercial
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboard()
    {
        return Ok(await _statistiqueService.GetDashboardStatsAsync());
    }

    // GET api/statistiques/ventes-par-periode?debut=2026-01-01&fin=2026-08-02&granularite=jour
    [HttpGet("ventes-par-periode")]
    public async Task<ActionResult<List<VenteParPeriodeDto>>> GetVentesParPeriode(
        [FromQuery] DateTime? debut,
        [FromQuery] DateTime? fin,
        [FromQuery] string granularite = "jour")
    {
        var dateFin = fin ?? DateTime.Now;
        var dateDebut = debut ?? dateFin.AddDays(-30);

        if (dateDebut > dateFin)
        {
            return BadRequest("La date de début doit précéder la date de fin.");
        }

        return Ok(await _statistiqueService.GetVentesParPeriodeAsync(dateDebut, dateFin, granularite));
    }

    // GET api/statistiques/top-produits?top=5
    [HttpGet("top-produits")]
    public async Task<ActionResult<List<TopProduitDto>>> GetTopProduits([FromQuery] int top = 5)
    {
        if (top < 1 || top > 100) top = 5;
        return Ok(await _statistiqueService.GetTopProduitsAsync(top));
    }

    // GET api/statistiques/top-clients?top=5
    [HttpGet("top-clients")]
    public async Task<ActionResult<List<TopClientDto>>> GetTopClients([FromQuery] int top = 5)
    {
        if (top < 1 || top > 100) top = 5;
        return Ok(await _statistiqueService.GetTopClientsAsync(top));
    }
}
