using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebService.Data;
using WebService.DTO;
using WebService.Models;
using System.Security.Cryptography;

namespace WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public AuthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO dto)
        {
            try
            {
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Username == dto.username && u.Password == dto.password);
                if (user == null)
                {
                    return Unauthorized("Неверное имя пользователя или пароль");
                }

                user.ApiKey = Guid.NewGuid().ToString();
                user.ApiKeyExpiration = DateTime.UtcNow.AddHours(2);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Message = "Успешный вход", ApiKey = user.ApiKey, Role = user.Role });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при логине: {ex.Message}");
                return StatusCode(500, new { Message = "Внутренняя ошибка сервера", Details = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Role) || string.IsNullOrEmpty(dto.Password))
                return BadRequest("Имя пользователя, роль и пароль обязательны");

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (existingUser != null)
                return BadRequest("Пользователь уже существует");

            Guid? groupId = null;
            if (!string.IsNullOrEmpty(dto.GroupName))
            {
                var group = await _dbContext.Groups.FirstOrDefaultAsync(g => g.Name == dto.GroupName);
                if (group == null)
                    return BadRequest("Группа с таким номером не найдена");
                groupId = group.GroupId;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Patronymic = dto.Patronymic,
                Username = dto.Username,
                Password = dto.Password,
                Email = dto.Email,
                Role = dto.Role,
                GroupId = groupId
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Пользователь зарегистрирован", UserId = user.Id });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] User dto)
        {
            var buffer = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (buffer == null)
            {

                
                buffer.Username = dto.Email;
                buffer.Email = dto.Email;
                buffer.Role = "student";
                buffer.Password = null;
                buffer.FirstName = dto.FirstName ?? "Google";
                buffer.LastName = dto.LastName ?? "User";
                buffer.Patronymic = dto.Patronymic;

                dto.Id = Guid.NewGuid();
                _dbContext.Users.Add(dto);
                await _dbContext.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.email);
            if (user == null)
                return NotFound("Пользователь не найден");

            if (string.IsNullOrEmpty(dto.password) || string.IsNullOrEmpty(dto.confirmpassword))
                return BadRequest("Пароль и подтверждение пароля должны быть указаны");

            if (dto.password != dto.confirmpassword)
                return BadRequest("Пароль и подтверждение пароля не совпадают");

            user.Password = dto.password;
            await _dbContext.SaveChangesAsync();
            return Ok(new { Message = "Пароль успешно сброшен" });
        }
    }
}