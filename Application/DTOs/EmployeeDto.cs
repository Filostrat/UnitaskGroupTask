namespace Application.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ManagerId { get; set; }
    public bool Enable { get; set; }

    public List<EmployeeDto> Children { get; set; } = new();
}