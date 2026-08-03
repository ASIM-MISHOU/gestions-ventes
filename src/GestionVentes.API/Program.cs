using GestionVentes.API.Data;
using GestionVentes.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration de SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=gestion_ventes.db"));

// Services métier (Paiements, Promotions, Historique, Statistiques)
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IStatistiqueService, StatistiqueService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();