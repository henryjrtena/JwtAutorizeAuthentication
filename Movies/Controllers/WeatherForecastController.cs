using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Movies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {
        private IConfiguration _config;
    
        public WeatherForecastController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("GenerateToken", Name = "GenerateToken")]
        public IActionResult GenerateToken()
        {
            try
            {
                var jwtSettingsSection = _config.GetSection("JwtSettings");


                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettingsSection.GetValue<string>("Key")));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "hatena"),
                    new Claim(ClaimTypes.Email, "hatena@gmail.com"),
                    new Claim(ClaimTypes.GivenName, "Henry"),
                    new Claim(ClaimTypes.Surname, "Tena"),
                    new Claim(ClaimTypes.Role, "AdminOnly")
                };

                var token = new JwtSecurityToken(
                    issuer: "https://localhost:7180/",  // Replace with your actual issuer URL
                    audience: "https://localhost:7180/",  // Replace with your actual audience URL
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(15),  // Token expiration time
                    signingCredentials: credentials
                );

                // Return the JWT token as a string
                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "Failed to generate token: " + ex.Message);
            }
        }

        [Authorize]
        [HttpGet("AmIAuthenticated", Name = "AmIAuthenticated")]
        public IActionResult AmIAuthenticated()
        {
            // This method will only be reached if the token is successfully validated
            return Ok("Authenticated");
        }

        [Authorize(Roles = "AdminOnly")]
        [HttpGet("AmIAdmin", Name = "AmIAdmin")]
        public IActionResult AmIAdmin()
        {
            // This method will only be reached if the token is successfully validated
            return Ok("I am admin");
        }
    }
}
