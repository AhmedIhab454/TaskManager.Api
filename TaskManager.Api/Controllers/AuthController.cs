using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Data; //=>App Context
using TaskManager.Api.DTOs; //=>RegisterDto
using TaskManager.Api.Models; //=>User Entity
using Microsoft.EntityFrameworkCore; //efcore methods
using System.Security.Cryptography; // password hashing 
using System.Text; //encoding 



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
    }
}
