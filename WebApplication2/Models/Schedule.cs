namespace WebService.Models
{
    public class Schedule
    {
        public Guid id { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public string Subject { get; set; } 
        public string Type { get; set; }
        public int ClassRoomId { get; set; }
        public ClassRoom ClassRoom { get; set; }
        public int TeacherId { get; set; }
        public User Teacher { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
