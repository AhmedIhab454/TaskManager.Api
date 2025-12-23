using Microsoft.AspNetCore.Authorization; // [Authorize]
using Microsoft.AspNetCore.Mvc; // ControllerBase
using Microsoft.EntityFrameworkCore; // EF Core
using System.Security.Claims; // Read user claims
using TaskManager.Api.Data; // DbContext
using TaskManager.Api.Models; // TaskItem model
namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public TaskController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet]
        public async Task<ActionResult<List<TaskItem>>> GetTasks()
        {
            string? userId= User.FindFirstValue(ClaimTypes.NameIdentifier);
            int ParsedUserId = int.Parse(userId!);
            var tasks = await _dbContext.TaskItems
                .Where(t => t.UserId == ParsedUserId)
                .ToListAsync();
            return Ok(tasks);
        }
    }
}
