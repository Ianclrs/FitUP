using FitUP;
using FitUP.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5000") });

// Serviços da aplicação
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PlanoTreinoService>();
builder.Services.AddScoped<BioimpedanciaService>();
builder.Services.AddScoped<PlanoAlimentarService>();

await builder.Build().RunAsync();
