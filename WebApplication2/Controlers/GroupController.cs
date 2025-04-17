using WebService.Data;
using WebService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebService.Data;
using WebService.Models;
using WebService.DTO;

namespace WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public GroupController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Group>> CreateGroup(Group group)
        {
            group.GroupId = Guid.NewGuid();
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGroup), new { id = group.GroupId }, group);
        }

        [HttpGet]
        [Authorize(Roles = "admin,teacher")]
        public async Task<ActionResult<IEnumerable<GroupDTO>>> GetGroup()
        {
            var groups = await _dbContext.Groups
                .Include(g => g.Students)
                .Select(g => new GroupDTO(
                    g.GroupId,
                    g.Name,
                    g.Students.Select(s => $"{s.FirstName} {s.LastName}").ToList()
                ))
                .ToListAsync();

            return Ok(groups);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteGroup(Guid id)
        {
            var group = await _dbContext.Groups
                .Include(g => g.Students)
                .FirstOrDefaultAsync(g => g.GroupId == id);

            if (group == null)
                return NotFound("Группа не найдена");

            foreach (var student in group.Students)
            {
                student.GroupId = null;
            }

            _dbContext.Groups.Remove(group);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Группа успешно удалена" });
        }
    }
}