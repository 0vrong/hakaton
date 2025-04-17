using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebService.Data;
using WebService.Models;
namespace WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public AnalyticsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("classroom/{classroomId}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<object>> GetClassroomUsage(Guid classroomId)
        {
            var schedules = await _dbContext.Schedules
                .Where(s => s.ClassRoomId == classroomId)
                .ToListAsync();
            return schedules;
        }

        [HttpGet("teacher/{teacherId}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<object>> GetTeacherWorkload(Guid teacherId)
        {
            var schedules = await _dbContext.Schedules
                .Where(s => s.TeacherId == teacherId)
                .ToListAsync();
            return schedules;
        }
    }
}