using TaskManagmentSystem.Data.Shared.Enums;

namespace TaskService.Data;

public class TaskModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<TaskSchedule>? TaskSchedules { get; set; }
    public List<TaskCompletion>? TaskCompletions { get; set; }
    public int TopicId { get; set; }
    public TopicModel? Topic { get; set; }
    public string? AssignedUserId { get; set; }
}
