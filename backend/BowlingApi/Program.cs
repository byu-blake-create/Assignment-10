using BowlingApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register API controllers so route endpoints like /api/bowlers are enabled.
builder.Services.AddControllers();
// Expose OpenAPI metadata in development.
builder.Services.AddOpenApi();

// Register EF Core context and point it at the local SQLite file.
builder.Services.AddDbContext<BowlingLeagueContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BowlingConnection")));

// Allow the React dev server to call this API during local development.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Map the OpenAPI endpoint while developing.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Apply the CORS policy before hitting controller endpoints.
app.UseCors("AllowReact");
app.UseHttpsRedirection();
app.UseAuthorization();
// Map controller actions to their HTTP routes.
app.MapControllers();
app.Run();
