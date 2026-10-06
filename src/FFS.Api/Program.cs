using FFS.Api;
using FFS.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFfsApplication();
builder.Services.AddSingleton<EmailOutbox>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "F.F.S API",
        Version = "v1",
        Description = "Mock finance calls and the future custom-email service. Data is in memory."
    });
});
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "F.F.S API");
    options.RoutePrefix = "swagger";
    options.InjectStylesheet("/swagger-ui/dark.css");
    options.InjectJavascript("/swagger-ui/theme-toggle.js");
});
app.MapControllers();
app.Run();
