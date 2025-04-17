using WebService.Models;

public class Schedule
{
    public Guid ScheduleId { get; set; }
    public string Lesson { get; set; } 
    public string Type { get; set; }
    public Guid TeacherId { get; set; }
    public User Teacher { get; set; } 
    public Guid ClassRoomId { get; set; }
    public ClassRoom ClassRoom { get; set; }
}