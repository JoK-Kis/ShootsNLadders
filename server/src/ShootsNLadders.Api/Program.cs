using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using ShootsNLadders.Core.Interfaces;
using ShootsNLadders.Core.Services;
using ShootsNLadders.Infrastructure.Data;
using ShootsNLadders.Infrastructure.Repositories;

// Load server/.env (searched for in this folder and its parents) into environment variables,
// before the builder reads configuration. NoClobber: real environment variables (e.g. set by
// systemd in production) win over the file.
Env.TraversePath().NoClobber().Load();

var builder = WebApplication.CreateBuilder(args);

// ---- Configuration ----

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings:Default is not configured. Copy server/.env.example to server/.env and fill it in.");
}

// ---- Services (what exists) ----

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database: one AppDbContext per request (scoped). IUnitOfWork must be that same instance.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

builder.Services.AddScoped<IEntryRepository, EntryRepository>();
builder.Services.AddScoped<IEntryService, EntryService>();

// The real clock: no state, safe to share, so one instance for the whole app.
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

// ---- Request pipeline (what happens to each request) ----

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // /openapi/v1.json, for importing into Bruno
}

app.MapControllers();

app.Run();
