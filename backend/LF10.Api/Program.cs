using System.Text.Json;
using HarassmentFilter.Core.Models;
using HarassmentFilter.Core.Services;

var builder = WebApplication.CreateBuilder(args);

var json = File.ReadAllText(
    Path.Combine(
        builder.Environment.ContentRootPath,
        "Data",
        "Harassment.json"));

var configuration =
    JsonSerializer.Deserialize<HarassmentFilterConfiguration>(
        json);

// Add services to the container.
builder.Services.AddSingleton<IHarassmentFilterService>(
    _ => new HarassmentFilterService(configuration));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();