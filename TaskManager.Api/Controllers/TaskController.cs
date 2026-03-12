using Microsoft.AspNetCore.Authorization; // [Authorize]
using Microsoft.AspNetCore.Mvc; // ControllerBase
using Microsoft.EntityFrameworkCore; // EF Core
using System.Security.Claims; // Read user claims
using TaskManager.Api.Data; // DbContext
using TaskManager.Api.DTOs;
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
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            //string? userId= User.FindFirstValue(ClaimTypes.NameIdentifier);
            // int ParsedUserId = int.Parse(userId!);
            var tasks = await _dbContext.TaskItems
                .Where(t => t.UserId == userId)
                .Select(t => new TaskResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted
                })
                .ToListAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponseDto>> GetTaskById(int id)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var task = await _dbContext.TaskItems
                .Where(t => t.Id == id && t.UserId == userId)
                .Select(t => new TaskResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted
                })
                .FirstOrDefaultAsync();
            if (task == null)
            {
                return NotFound("Task not found.");
            }
            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var task = await _dbContext.TaskItems
                .Where(t => t.Id == id && t.UserId == userId)
                .FirstOrDefaultAsync();
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            task.Title = updateTaskDto.Title;
            task.IsCompleted = updateTaskDto.IsCompleted;
            task.Description = updateTaskDto.Description;
            await _dbContext.SaveChangesAsync();
            return Ok(new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var task = await _dbContext.TaskItems
                .Where(t => t.Id == id && t.UserId == userId)
                .FirstOrDefaultAsync();
            if (task == null)
            {
                return NotFound("Task not found.");
            }
            _dbContext.TaskItems.Remove(task);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
           // string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            //if (userId == null)
            //{
            //    return Unauthorized();
            //}
            //int parsedUserId = int.Parse(userId);
            var newTask = new TaskItem
            {
                Title = createTaskDto.Title,
                IsCompleted = false,
                UserId = userId,
                Description = createTaskDto.Description
            };
            _dbContext.TaskItems.Add(newTask);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTaskById), new { id = newTask.Id }, new TaskResponseDto
            {
                Id = newTask.Id,
                Title = newTask.Title,
                IsCompleted = newTask.IsCompleted,
                Description = newTask.Description
            });
        }

    }
}
