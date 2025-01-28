using TaskService.Data;

namespace TaskService.Repositories.Tasks;

public interface ITaskRepository : IGenericRepository<TaskModel>
{
    Task<IEnumerable<TaskModel>> GetTasksByUserId(string userId);
    Task<TaskModel?> GetTaskByUserId(int id);
    Task<IEnumerable<TaskModel>> GetTasksByDate(string userId);
    Task<TopicModel?> GetTopicForColor(int id);
}