namespace WebService.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }
        public string Role { get; set; }

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