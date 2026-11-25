using HarassmentFilter.Core.Interfaces;
using HarassmentFilter.Core.Services;
using LF10.Api.Hubs;
using LF10.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDatabase(builder.Configuration.GetConnectionString("Default"));
builder.Services.AddHarassmentFilter();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

var app = builder.Build();

// The API tests switch this off, they run without MySQL.
if (app.Configuration.GetValue("Database:MigrateOnStartup", true))
{
    app.Services.MigrateDatabase();
}

// Load the filter rules now, so a database problem stops the startup
// instead of failing the first chat message.
app.Services.GetRequiredService<IFilterService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "HugYou API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chathub");

app.Run();
