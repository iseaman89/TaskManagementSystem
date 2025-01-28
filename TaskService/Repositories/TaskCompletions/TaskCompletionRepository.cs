using TaskService.Data;

namespace TaskService.Repositories.TaskCompletions;

public class TaskCompletionRepository : GenericRepository<TaskCompletion>, ITaskCompletionRepository
{
    private readonly TaskDbContext _context;

    public TaskCompletionRepository(TaskDbContext context) : base(context)
    {
        _context = context;
    }
}