using ApexCharts;
using FFS.Application;
using FFS.Web.Components;
using FFS.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFfsApplication();
builder.Services.AddApexCharts();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<AppBootService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
