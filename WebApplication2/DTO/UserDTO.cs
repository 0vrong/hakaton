namespace WebService.DTO
{
    public record CreateUserDTO(
        string FirstName,
        string LastName,
        string Patronymic,
        string Username,
        string Password,
        string? Email,
        string Role,
        string? GroupName
    );
}