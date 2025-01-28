using TaskService.Data;

namespace TaskService.Repositories.Topics;

public interface ITopicRepository : IGenericRepository<TopicModel>
{
    Task<IEnumerable<TopicModel>> GetTopicsByUserIdAsync(string userId);
}