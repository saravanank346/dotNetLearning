// Import your DbContext (EF Core database class)
using System.Text;
using dotNetAPi.Data;

// Import your custom services (business logic)
using dotNetAPi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

// Import EF Core (needed for DB connection)
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


// 🔥 Create application builder (this is the starting point of ASP.NET app)
var builder = WebApplication.CreateBuilder(args);


// ================== ADD SERVICES (Dependency Injection) ==================

// Add support for Controllers (API endpoints)
builder.Services.AddControllers();

// Add API explorer (used by Swagger to understand endpoints)
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Basic swagger info
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "dotNetAPi",
        Version = "v1"
    });

    // Add JWT auth definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter token like: Bearer your_token"
    });

    // Tell swagger to use this auth definition
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



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
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<UserService>(); // handles user logic
builder.Services.AddScoped<JwtService>();  // handles JWT token creation
builder.Services.AddScoped<AuthService>(); // handles login/auth logic

// ✅ IMPORTANT: set default authentication scheme
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});

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