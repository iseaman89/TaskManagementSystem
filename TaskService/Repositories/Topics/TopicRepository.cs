using Microsoft.EntityFrameworkCore;
using TaskService.Data;

namespace TaskService.Repositories.Topics;

public class TopicRepository : GenericRepository<TopicModel>, ITopicRepository
{
    private readonly TaskDbContext _context;
    public TopicRepository(TaskDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<TopicModel>> GetTopicsByUserIdAsync(string userId)
    {
        return await _context.Topics
            .Where(t => t.AssignedUserId == userId)
            .Include(t => t.Tasks)!
            .ThenInclude(task => task.TaskSchedules)
            .Include(t => t.Tasks)!
            .ThenInclude(task => task.TaskCompletions)
            .ToListAsync();
    }
}