using TaskService.Data;

namespace TaskService.Repositories.TaskStatistics;

public interface ITaskStatisticRepository : IGenericRepository<TaskModel>
{
    Task<IEnumerable<TopicModel>> GetTopicsByUserId(string userId);
}