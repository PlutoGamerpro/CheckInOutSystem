        using Microsoft.EntityFrameworkCore; // manter única ocorrência
using Npgsql.EntityFrameworkCore.PostgreSQL; // necessário para extensão UseNpgsql
using TimeRegistration.Data;

// cspell:ignore Npgsql Tillad Brug policyen altid ikke

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRepositories().AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS: Tillad kun Angular dev-server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSingleton<TimeRegistration.Services.IAdminAuthService, TimeRegistration.Services.AdminAuthService>();

var app = builder.Build();

// Brug CORS-policyen altid (ikke kun i development)
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
