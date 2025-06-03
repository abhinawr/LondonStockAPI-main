
using LondonStockApi.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LondonStockApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a JWT token for valid demo user credentials.
        /// </summary>
        [AllowAnonymous] // This endpoint does not require authentication
        [HttpPost("token")]
        [ProducesResponseType(typeof(TokenResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status401Unauthorized)]
        public IActionResult GenerateToken([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorViewModel("Invalid login data.", ModelState.ToString()));
            }

            // --- DEMO USER VALIDATION ---
            // In a real app, use ASP.NET Core Identity or a proper user store with hashed passwords
            var demoUsers = _configuration.GetSection("DemoUsers").Get<List<LoginModel>>();
            var user = demoUsers?.FirstOrDefault(u =>
                u.Username.Equals(loginModel.Username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == loginModel.Password); // DO NOT compare passwords like this in production!

            if (user == null)
            {
                return Unauthorized(new ErrorViewModel("Invalid username or password."));
            }
            // --- END DEMO USER VALIDATION ---

            var jwtSettings = _configuration.GetSection("Jwt");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured.")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
                new Claim(ClaimTypes.NameIdentifier, user.Username), 
                // Add other claims as needed (e.g., roles)
                new Claim("broker_id", user.Username) // Custom claim for broker ID
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"] ?? "60")),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new TokenResponseModel
            {
                Token = tokenHandler.WriteToken(token),
                Expiry = token.ValidTo
            });
        }
    }
}