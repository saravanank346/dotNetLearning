// Import basic .NET features like DateTime, Exception, Console
using System;

// Import your EF Core DbContext class
using dotNetAPi.Data;

// Import DTO classes like Iuser, Ilogin, IloginResponse
using dotNetAPi.Dtos;

// Import User model/entity
using dotNetAPi.Models;

// Import EF Core async methods like FirstOrDefaultAsync, ToListAsync, FindAsync
using Microsoft.EntityFrameworkCore;

namespace dotNetAPi.Services
{
    // UserService contains business logic related to users
    public class UserService
    {
        // Private field to access database through EF Core
        private readonly AppDbContext _context;

        // Private field to generate JWT token
        private readonly JwtService _jwtService;

        // Private field to get user permissions
        private readonly AuthService _authService;

        // Constructor
        // Dependency Injection automatically provides these objects
        public UserService(AppDbContext context, JwtService jwtService, AuthService authService)
        {
            // Store injected AppDbContext inside _context
            _context = context;

            // Store injected JwtService inside _jwtService
            _jwtService = jwtService;

            // Store injected AuthService inside _authService
            _authService = authService;
        }

        // Register new user
        // Input: Iuser dto
        // Output: created User object
        public async Task<User?> RegisterUser(Iuser dto)
        {
            // Create new User object from incoming dto data
            var user = new User
            {
                // Set Name from request dto
                Name = dto.Name,

                // Set Email from request dto
                Email = dto.Email,

                // Hash password before saving to DB
                // Never store plain password directly
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),

                // Set created time in UTC
                Created_At = DateTime.UtcNow,
            };

            // Add new user to EF Core tracking
            _context.Users.Add(user);

            // Save changes to database
            await _context.SaveChangesAsync();

            // Return created user
            return user;
        }

        // Login method
        // Input: login email and password
        // Output: login response with user data, token, permissions
        public async Task<IloginResponse> loginUser(Ilogin iuser)
        {
            // This old code returns all matching users
            // Usually email should be unique, so not needed
            // var user = await _context.Users.Where(x => x.Email == iuser.Email).ToListAsync();

            // Find first user whose email matches login email
            // If no user found, returns null
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == iuser.Email);

            // Print user value in console for debugging
            Console.WriteLine(user + "user......");

            // If user not found, throw exception
            if (user == null)
            {
                throw new Exception("Email not found");
            }

            // Compare entered password with hashed password stored in DB
            var verifyHashPassword = BCrypt.Net.BCrypt.Verify(iuser.Password, user.Password);

            // If password is wrong, throw exception
            if (!verifyHashPassword)
            {
                throw new Exception("Email or Password Invalid");
            }

            int roleId = await _authService.getRoleIdByUserId(user.Id);
            Console.WriteLine(roleId + ".....roleId");

            // Get permissions for this user from AuthService
            var permissions = await _authService.GetUserRolePermission(roleId);

            // Generate JWT token using user data + permissions
            var token = _jwtService.GenerateJwtToken(user, permissions, roleId);

            // Return login response object
            return new IloginResponse
            {
                // user data
                Data = user,

                // generated JWT token
                accessToken = token,

                // list of permissions
                permissions = permissions,
            };
        }

        // Get all users
        // Output: list of all users from Users table
        public async Task<List<User>> getAllUsers()
        {
            // Fetch all rows from Users table
            var getAllResult = await _context.Users.ToListAsync();

            // Return list
            return getAllResult;
        }

        // Get single user by id
        // Input: user id
        // Output: matching user
        public async Task<User> getUserById(int id)
        {
            // Find user by primary key id
            var result = await _context.Users.FindAsync(id);

            // Return result
            // result! means "I believe result is not null"
            return result!;
        }

        // Update user by id
        // Input: id + new dto data
        // Output: updated user object
        public async Task<User> updateByUserId(int id, Iuser dto)
        {
            // Find user by id
            var userById = await _context.Users.FindAsync(id);

            // If user not found, throw exception
            if (userById == null)
            {
                throw new Exception("User not found ..");
            }

            // Update name
            userById.Name = dto.Name;

            // Update email
            userById.Email = dto.Email;

            // Hash new password before saving
            userById.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Set updated time
            userById.Updated_At = DateTime.UtcNow;

            // Save updated values in DB
            await _context.SaveChangesAsync();

            // Return updated user
            return userById;
        }
    }
}