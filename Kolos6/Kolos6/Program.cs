using Kolos6;
using Kolos6.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 📌 Konfiguracja Swaggera (dokumentacja API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 📌 Rejestracja DbContext (Database First)
builder.Services.AddDbContext<MasterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 📌 Rejestracja serwisów aplikacji
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IClientService, ClientService>();

var app = builder.Build();

// 📌 Włączenie Swaggera w trybie developerskim
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 📌 Rejestracja endpointów API
app.RegisterEndpoints();

app.UseHttpsRedirection();
app.Run();