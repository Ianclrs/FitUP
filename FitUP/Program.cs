using FitUP;
using FitUP.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5000";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// HttpClient para carregar arquivos estáticos do wwwroot (JSONs de dados)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Serviços da aplicação
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PlanoTreinoService>();
builder.Services.AddScoped<BioimpedanciaService>();
builder.Services.AddScoped<PlanoAlimentarService>();
builder.Services.AddScoped<ExerciseCatalogService>();
builder.Services.AddSingleton<DietaDataService>();

await builder.Build().RunAsync();
