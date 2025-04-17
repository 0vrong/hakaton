using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebService.Data;
using WebService.DTO;
using WebService.Models;

namespace WebService.Controllers
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
        public async Task<ActionResult<IEnumerable<ScheduleDTO>>> GetSchedules()
        {
            var schedules = await _dbContext.Schedules
                .Include(s => s.Teacher)
                .Include(s => s.ClassRoom)
                .Select(s => new ScheduleDTO(
                    s.Lesson,
                    s.Teacher.LastName,
                    s.ClassRoom.Number,
                    s.Type
                ))
                .ToListAsync();

            return Ok(schedules);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin,teacher,student")]
        public async Task<ActionResult<ScheduleDTO>> GetSchedule(Guid id)
        {
            var schedule = await _dbContext.Schedules
                .Include(s => s.Teacher)
                .Include(s => s.ClassRoom)
                .Select(s => new ScheduleDTO(
                    s.Lesson,
                    s.Teacher.LastName,
                    s.ClassRoom.Number,
                    s.Type
                ))
                .FirstOrDefaultAsync(s => s.Lesson == id.ToString());

            if (schedule == null)
                return NotFound("Расписание не найдено");

            return Ok(schedule);
        }

        [HttpPost]
        [Authorize(Roles = "admin,teacher")]
        public async Task<IActionResult> CreateSchedule([FromBody] ScheduleDTO dto)
        {
            var teacher = await _dbContext.Users.FirstOrDefaultAsync(u => u.LastName == dto.TeacherSurname && u.Role == "teacher");
            if (teacher == null)
                return BadRequest("Преподаватель не найден");

            var classRoom = await _dbContext.ClassRooms.FirstOrDefaultAsync(c => c.Number == dto.ClassRoomNumber);
            if (classRoom == null)
                return BadRequest("Кабинет не найден");

            var schedule = new Schedule
            {
                ScheduleId = Guid.NewGuid(),
                Lesson = dto.Lesson,
                Type = dto.TypeLesson,
                TeacherId = teacher.Id,
                ClassRoomId = classRoom.Id
            };

            _dbContext.Schedules.Add(schedule);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSchedule), new { id = schedule.ScheduleId }, dto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateSchedule(Guid id, [FromBody] ScheduleDTO dto)
        {
            var schedule = await _dbContext.Schedules.FindAsync(id);
            if (schedule == null)
                return NotFound("Расписание не найдено");

            var teacher = await _dbContext.Users.FirstOrDefaultAsync(u => u.LastName == dto.TeacherSurname && u.Role == "teacher");
            if (teacher == null)
                return BadRequest("Преподаватель не найден");

            var classRoom = await _dbContext.ClassRooms.FirstOrDefaultAsync(c => c.Number == dto.ClassRoomNumber);
            if (classRoom == null)
                return BadRequest("Кабинет не найден");

            schedule.Lesson = dto.Lesson;
            schedule.Type = dto.TypeLesson;
            schedule.TeacherId = teacher.Id;
            schedule.ClassRoomId = classRoom.Id;

            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Расписание обновлено" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteSchedule(Guid id)
        {
            var schedule = await _dbContext.Schedules.FindAsync(id);
            if (schedule == null)
                return NotFound("Расписание не найдено");

            _dbContext.Schedules.Remove(schedule);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}