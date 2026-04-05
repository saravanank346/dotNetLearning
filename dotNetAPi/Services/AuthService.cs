using System;

// Import DbContext (EF Core database class)
using dotNetAPi.Data;

// Import EF Core (for async queries like ToListAsync)
using Microsoft.EntityFrameworkCore;

namespace dotNetAPi.Services
{
    // 🔥 AuthService → Handles authentication & authorization logic
    public class AuthService
    {
        // 🔹 Private variable to access database
        private readonly AppDbContext _appDBConetxt;

        // 🔹 Constructor (Dependency Injection)
        // .NET will automatically give AppDbContext instance
        public AuthService(AppDbContext appDbContext)
        {
            _appDBConetxt = appDbContext;
        }

        // 🔥 Method: Get permissions for a user
        // Input: userId
        // Output: List of permissions (like "User:Create", "Report:View")
        public async Task<List<string>?> GetUserRolePermission(int roleId)
        {
            // 🔥 LINQ Query (EF Core → converted to SQL)
            var permissions = await (

                // Step 1: Start from UserRoleMappings (User → Role)
                from usrm in _appDBConetxt.UserRoleMappings

                    // Step 2: Join RoleActionFeatureMappings (Role → Feature + Action)
                join rafm in _appDBConetxt.RoleActionFeatureMappings
                on usrm.RoleId equals rafm.RoleId

                // Step 3: Join Action table (get action name like Create, Edit)
                join action in _appDBConetxt.Action
                on rafm.ActionId equals action.Id

                // Step 4: Join Feature table (get feature name like User, Report)
                join feature in _appDBConetxt.Features
                on rafm.FeatureId equals feature.Id

                // Step 5: Filter by userId
                where usrm.RoleId == roleId

                // Step 6: Create permission string
                // Example: "User:Create"
                select feature.ActionName + ":" + action.ActionName

                )
                .ToListAsync(); // execute query in DB

            // Return list of permissions
            return permissions;
        }

        #region getRoleIdByUserId

        public async Task<int> getRoleIdByUserId(int id)
        {
            var userDetails = await _appDBConetxt.UserRoleMappings.FirstOrDefaultAsync(x=>x.UserId==id);
            Console.WriteLine(userDetails + ".....userDetails");

            if(userDetails == null)
            {
                throw new Exception("User Doesnot have role!.");
            }

            int roleId = userDetails.RoleId;
            Console.WriteLine(userDetails.RoleId + ".....roleId.....");

            return roleId;
        }

        #endregion
    }
}