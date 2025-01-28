using Microsoft.EntityFrameworkCore;
using TaskService.Data;

namespace TaskService.Repositories.TaskStatistics;

public class TaskStatisticRepository : GenericRepository<TaskModel>, ITaskStatisticRepository
{
    private readonly TaskDbContext _context;

    public TaskStatisticRepository(TaskDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TopicModel>> GetTopicsByUserId(string userId)
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