using System;

// JWT token classes (create & handle tokens)
using System.IdentityModel.Tokens.Jwt;

// For claims (data inside token)
using System.Security.Claims;

// For encoding key to bytes
using System.Text;

// Your User model
using dotNetAPi.Models;

// 🔥 Important for token security (key + algorithm)
using Microsoft.IdentityModel.Tokens;

namespace dotNetAPi.Services
{
    public class JwtService
    {
        // 🔹 Used to read values from appsettings.json
        private readonly IConfiguration _configuration;

        // 🔹 Constructor (Dependency Injection)
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 🔥 Method to generate JWT token
        // Input: User + permissions
        // Output: Token string
        public string GenerateJwtToken(User user, List<string>? permissions)
        {
            // 🔹 Read config values from appsettings.json
            var key = _configuration["jwt:key"];          // secret key
            var Issuer = _configuration["jwt:Issuer"];    // who created token
            var Audience = _configuration["jwt:Audience"]; // who can use token
            var Expiry = Convert.ToDouble(_configuration["jwt:Expiry"]); // expiry time


            // 🔥 Claims = data stored inside token
            var claims = new List<Claim>
            {
                // user id
				new Claim("id", user.Id.ToString()),

                // user name
				new Claim("name", user.Name!),

                // user email
				new Claim("email", user.Email!)
            };


            // 🔥 Add permissions to token
            // Example: "User:Create", "Report:View"
            foreach (var permission in permissions!)
            {
                claims.Add(new Claim("permission", permission));
            }


            // 🔥 Create security key (convert string → byte array)
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key!)
            );


            // 🔥 Define how token is signed
            var signingCreditionals = new SigningCredentials(
                securityKey,                    // secret key
                SecurityAlgorithms.HmacSha256   // algorithm
            );


            // 🔥 Create JWT token
            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Expiry),
                signingCredentials: signingCreditionals
            );


            // 🔥 Convert token object → string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}