namespace WebService.DTO
{
    public record ScheduleDTO(
        string Lesson,         
        string TeacherSurname, 
        string ClassRoomNumber,
        string TypeLesson
    );
}