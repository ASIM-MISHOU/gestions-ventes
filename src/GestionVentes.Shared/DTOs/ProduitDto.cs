namespace GestionVentes.Shared.DTOs;

public class ProduitDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public int QuantiteStock { get; set; }
}

public class CreateProduitDto
{
    public string Nom { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public int QuantiteStock { get; set; }
}