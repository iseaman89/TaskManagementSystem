namespace TaskService.Models.Statistics;

public class TopicStatistic : BaseStatistic
{
    public string TopicTitel { get; set; }
    public string Color { get; set; }
    public List<TaskStatistic> TaskStatistics { get; set; }
}