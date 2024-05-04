using DataAccess;
using DataAccess.Migrations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddDataAccess("Host=localhost;Port=5432;Username=postgres;Password=postgres");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

WebApplication app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();
scope.ResetDataBase();
scope.SetUpDataBase();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();