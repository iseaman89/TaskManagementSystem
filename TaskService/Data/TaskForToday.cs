using TaskManagmentSystem.Data.Shared.Enums;

namespace TaskService.Data;

public class TaskForToday
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public TaskPriority Priority { get; set; }
    public string? Color { get; set; }
}