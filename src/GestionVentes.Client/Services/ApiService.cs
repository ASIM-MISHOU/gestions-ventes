using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GestionVentes.Shared.DTOs;

namespace GestionVentes.Client.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(string baseUrl = "http://localhost:5000")
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    // ==========================================
    // MODULE PRODUITS
    // ==========================================

    public async Task<List<ProduitDto>?> GetProduitsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<ProduitDto>>("api/produits");
    }

    public async Task<ProduitDto?> GetProduitByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<ProduitDto>($"api/produits/{id}");
    }

    public async Task<ProduitDto?> CreateProduitAsync(CreateProduitDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/produits", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProduitDto>();
    }

    public async Task<bool> UpdateProduitAsync(int id, CreateProduitDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/produits/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteProduitAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/produits/{id}");
        return response.IsSuccessStatusCode;
    }

    // ==========================================
    // MODULE CLIENTS
    // ==========================================

    public async Task<List<ClientDto>?> GetClientsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<ClientDto>>("api/clients");
    }

    public async Task<ClientDto?> CreateClientAsync(CreateClientDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/clients", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ClientDto>();
    }

    // ==========================================
    // MODULE VENTES & FACTURES
    // ==========================================

    public async Task<List<VenteDto>?> GetVentesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<VenteDto>>("api/ventes");
    }

    public async Task<VenteDto?> CreateVenteAsync(CreateVenteDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/ventes", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<VenteDto>();
    }

    public async Task<List<FactureDto>?> GetFacturesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<FactureDto>>("api/factures");
    }

    public async Task<bool> PayerFactureAsync(int factureId, CreatePaiementDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/factures/{factureId}/paiements", dto);
        return response.IsSuccessStatusCode;
    }
}