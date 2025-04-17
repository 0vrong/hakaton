namespace WebService.DTO
{   
    public record GroupDTO(
    Guid GroupId,
    string Name,
    List<string> StudentNames
    );
}
