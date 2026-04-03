// Import basic .NET functionalities (not heavily used here, but standard)
using System;

// Import application models (User, Feature, etc.)
using dotNetAPi.Models;

// Import RBAC-related models (Action, mappings, etc.)
using dotNetAPi.Models.RBAC;

// 🔥 Import EF Core library (VERY IMPORTANT)
// Gives access to DbContext, DbSet, database operations
using Microsoft.EntityFrameworkCore;

// Define namespace (grouping this file under Data layer)
namespace dotNetAPi.Data
{
    // 🔥 AppDbContext is the main EF Core class that connects your app to DB
    // It inherits from DbContext (EF Core base class)
    public class AppDbContext : DbContext
    {
        // 🔹 Constructor
        // EF Core will pass database configuration (connection string, provider)
        // through DbContextOptions
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // base(options) → passes DB config to EF Core
        }

        // 🔥 DbSet<User> represents "Users" table in database
        // Each row in table = one User object
        public DbSet<User> Users { get; set; }

        // 🔥 Represents "Action" table (RBAC)
        // Example values: Create, Read, Update, Delete
        // Full namespace used to avoid conflict with System.Action
        public DbSet<dotNetAPi.Models.RBAC.Action> Action { get; set; }

        // 🔥 Represents "Features" table
        // Example: User Management, Dashboard, Reports
        public DbSet<Feature> Features { get; set; }

        // 🔥 Mapping table: User ↔ Role
        // Stores which user has which role
        public DbSet<UserRoleMapping> UserRoleMappings { get; set; }

        // 🔥 Mapping table: Role ↔ Feature ↔ Action
        // This is the core RBAC permission table
        // Defines what a role can do on a feature
        public DbSet<RoleActionFeatureMapping> RoleActionFeatureMappings { get; set; }
    }
}