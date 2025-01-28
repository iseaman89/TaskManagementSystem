namespace TaskService.Models.Statistics;

public class BaseStatistic
{
    public int? CompletedTasks { get; set; }
    public int? Schedules { get; set; }
    public int CompletionRate { get; set; }
}