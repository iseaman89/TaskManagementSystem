namespace TaskService.Data;

public class TopicModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Color { get; set; }
    public string[]? Tags { get; set; }
    public List<TaskModel>? Tasks { get; set; }
    public string? AssignedUserId { get; set; }
}