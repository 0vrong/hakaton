using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebService.Data;
using WebService.Models;

namespace WebService.Controlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ScheduleController(AppDbContext dbContext)
        {
                _dbContext = dbContext;
        }


        [HttpGet]
        [Authorize(Roles = "admin,teacher,student")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetSchedules()
        {
            return await _dbContext.Schedules
                .Include(s => s.Group)
                .Include(s => s.ClassRoom)
                .Include(s => s.Teacher)
                .ToListAsync();
        }



    }
}
