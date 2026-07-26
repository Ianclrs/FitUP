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

// Singleton: fonte única de verdade para o token JWT
// (compartilhado entre AuthService e AuthHeaderHandler)
builder.Services.AddSingleton<ITokenProvider, TokenProvider>();

// HttpClient nomeado para a API (usado pelos Services)
// O AuthHeaderHandler injeta automaticamente o token JWT em cada requisição
builder.Services.AddTransient<AuthHeaderHandler>();

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<AuthHeaderHandler>();

// HttpClient padrão para carregar arquivos estáticos do wwwroot (JSONs de dados, imagens)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Serviços da aplicação
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PlanoTreinoService>();
builder.Services.AddScoped<BioimpedanciaService>();
builder.Services.AddScoped<PlanoAlimentarService>();
builder.Services.AddScoped<ExerciseCatalogService>();
builder.Services.AddSingleton<DietaDataService>();

await builder.Build().RunAsync();
