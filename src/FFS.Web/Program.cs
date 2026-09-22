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

await builder.Build().RunAsync();
