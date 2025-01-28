namespace TaskService.Data;

public class TaskCompletion
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public TaskModel? Task { get; set; }
    public DateTime? CompletionDate { get; set; }
}