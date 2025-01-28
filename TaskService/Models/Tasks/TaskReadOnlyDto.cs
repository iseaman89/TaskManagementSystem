using TaskManagmentSystem.Data.Shared.Enums;
using TaskService.Data;

namespace TaskService.Models.Tasks;

public class TaskReadOnlyDto : BaseDto
{
    public string Title { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<TaskSchedule>? TaskSchedules { get; set; }
    public List<TaskCompletion>? TaskCompletions { get; set; }
    public int TopicId { get; set; }
    public string? AssignedUserId { get; set; }
}