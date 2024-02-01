global using Mawhiba.API.Gateway.Helpers;
global using Newtonsoft.Json;
global using Mawhiba.API.Gateway;
using Mawhiba.API.Gateway.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<ContentServicesDbContext>(options => options.UseSqlServer(connectionString));


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGatewayServices();
builder.Services.AddScoped<MyCustomMiddleware>();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<MyCustomMiddleware>();
app.MapControllers();

app.Run();
