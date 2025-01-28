using Microsoft.EntityFrameworkCore;
using TaskService.Data;

namespace TaskService.Repositories.Tasks;

public class TaskRepository : GenericRepository<TaskModel>, ITaskRepository
{
    private readonly TaskDbContext _context;
    public TaskRepository(TaskDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<TaskModel>> GetTasksByUserId(string userId)
    {
        return await _context.Tasks.Where(t => t.AssignedUserId == userId)
            .Include(t => t.TaskSchedules)
            .Include(t => t.TaskCompletions)
            .ToListAsync();
    }
    
    public async Task<TaskModel?> GetTaskByUserId(int id)
    {
        return await _context.Tasks.Where(t => t != null && t.Id == id)
            .Include(t => t!.TaskSchedules)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TaskModel>> GetTasksByDate(string userId)
    {
        return await _context.Tasks.Where(t => t.AssignedUserId == userId)
            .Include(t => t.TaskSchedules)
            .Include(t => t.TaskCompletions)
            .Where(t => t.TaskSchedules.Any(ts => ts.DayOfWeek == DateTime.Today.DayOfWeek))
            .Where(t => t.TaskCompletions == null || t.TaskCompletions.Any(tc => tc.CompletionDate.Value.Date != DateTime.UtcNow.Date))
            .ToListAsync();
    }

    public async Task<TopicModel?> GetTopicForColor(int id)
    {
        return await _context.Topics.Where(t => t.Id == id)
            .FirstOrDefaultAsync();
    }
}