using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TruckLoadingApp.Blazor;
using TruckLoadingApp.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
// Add SignalR Service
builder.Services.AddSingleton<TruckLocationService>();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
