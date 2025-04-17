using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebService.Data;
using WebService.Models;

namespace WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ClassroomController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ClassRoom>> Post(ClassRoom dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.Id = Guid.NewGuid();
            _dbContext.ClassRooms.Add(dto);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClassroom), new { id = dto.Id }, dto);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin,teacher,student")]
        public async Task<ActionResult<ClassRoom>> GetClassroom(Guid id)
        {
            var classroom = await _dbContext.ClassRooms.FindAsync(id);
            if (classroom == null)
                return NotFound();
            return classroom;
        }
    }
}