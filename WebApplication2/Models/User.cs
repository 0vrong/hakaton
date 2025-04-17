namespace WebService.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }
        public string Role { get; set; }
        public string? ApiKey { get; set; }
        public DateTime? ApiKeyExpiration { get; set; }
        public Guid? GroupId { get; set; }
        public Group? Group { get; set; }

        public bool HasPermission(string action)
        {
            return Role switch
            {  
                "admin" => true,
                "teacher" => action == "view_schedule" || action == "view_calendar",
                "student" => action == "view_schedule",
                _ => false
            };
        }
    }
}