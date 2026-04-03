// Import your DbContext (EF Core database class)
using dotNetAPi.Data;

// Import your custom services (business logic)
using dotNetAPi.Services;

// Import EF Core (needed for DB connection)
using Microsoft.EntityFrameworkCore;


// 🔥 Create application builder (this is the starting point of ASP.NET app)
var builder = WebApplication.CreateBuilder(args);


// ================== ADD SERVICES (Dependency Injection) ==================

// Add support for Controllers (API endpoints)
builder.Services.AddControllers();

// Add API explorer (used by Swagger to understand endpoints)
builder.Services.AddEndpointsApiExplorer();

// Add Swagger (API documentation UI)
builder.Services.AddSwaggerGen();


// ================== DATABASE CONFIG (EF CORE) ==================

// 🔥 Register AppDbContext with EF Core
// Use PostgreSQL (UseNpgsql)
// Get connection string from appsettings.json → "DefaultConnection"
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);


// ================== CUSTOM SERVICES ==================

// 🔥 Add services to Dependency Injection container

// Scoped = new instance per request

builder.Services.AddScoped<UserService>(); // handles user logic
builder.Services.AddScoped<JwtService>();  // handles JWT token creation
builder.Services.AddScoped<AuthService>(); // handles login/auth logic


// ================== BUILD APPLICATION ==================

// Build the app (combine all configs)
var app = builder.Build();


// ================== MIDDLEWARE PIPELINE ==================

// If environment = Development → enable Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();     // generate Swagger JSON
    app.UseSwaggerUI();   // open Swagger UI in browser
}


// Redirect HTTP → HTTPS
app.UseHttpsRedirection();


// 🔥 Authorization middleware
// Checks user permissions before accessing endpoints
app.UseAuthorization();


// Map controllers (connect routes to controller methods)
app.MapControllers();


// Run the application
app.Run();