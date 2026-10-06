using ApexCharts;
using FFS.Application;
using FFS.Web.Components;
using FFS.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddFfsApplication();
builder.Services.AddApexCharts();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<AppBootService>();
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
