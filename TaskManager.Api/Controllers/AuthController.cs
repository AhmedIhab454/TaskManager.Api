using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Data; //=>App Context
using TaskManager.Api.DTOs; //=>RegisterDto
using TaskManager.Api.Models; //=>User Entity
using Microsoft.EntityFrameworkCore; //efcore methods
using System.Security.Cryptography; // password hashing 
using System.Text; //encoding 
using Microsoft.IdentityModel.Tokens; // JWT
using System.IdentityModel.Tokens.Jwt; // JWT
using System.Security.Claims; // JWT Claims


namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        //DataBAse context
        private readonly AppDbContext _dbContext;


        //Dependency Injection
        public AuthController(AppDbContext appContext)
        {
            _dbContext = appContext;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            bool UserExists = await _dbContext.Users.AnyAsync(u => u.Username == registerDto.UsreName);
            if (UserExists)
            {
                return BadRequest("Username already exists.");
            }
            // Hash the password
            using var sha256 = SHA256.Create(); //Hasher
            byte[] passwordBytes = Encoding.UTF8.GetBytes(registerDto.Password); // convert to bytes
            byte[] hashedBytes = sha256.ComputeHash(passwordBytes); //hashing
            string passwordHash = Convert.ToBase64String(hashedBytes); //convert to string

            //Create new user
            var user = new User
            {
                Username = registerDto.UsreName,
                PasswordHash = passwordHash
            };

            //add to dbcontext
            _dbContext.Users.Add(user);
            //save to database
            await _dbContext.SaveChangesAsync();
            return Ok("User registered successfully.");

        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == loginDto.UsreName);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Hash the provided password   
            using var sha256 = SHA256.Create();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(loginDto.Password);
            byte[] hashedBytes = sha256.ComputeHash(passwordBytes);
            string passwordHash = Convert.ToBase64String(hashedBytes);
            // Compare hashes
            if (user.PasswordHash != passwordHash)
            {
                return Unauthorized("Invalid username or password.");
            }

            //create claims
            var claims = new[]
            {
                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User ID
                 new Claim(ClaimTypes.Name, user.Username) // Username
            };

            var key = new SymmetricSecurityKey(
                   Encoding.UTF8.GetBytes(
                       HttpContext.RequestServices
                           .GetRequiredService<IConfiguration>()
                           ["JwtSettings:SecretKey"]!
                   )
               );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: HttpContext.RequestServices
                           .GetRequiredService<IConfiguration>()
                           ["JwtSettings:Issuer"],
                audience: HttpContext.RequestServices
                           .GetRequiredService<IConfiguration>()
                           ["JwtSettings:Audience"],
                claims: claims,
                       expires: DateTime.UtcNow.AddMinutes(
            int.Parse(
                HttpContext.RequestServices.GetRequiredService<IConfiguration>()["JwtSettings:ExpiryMinutes"]! )),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
