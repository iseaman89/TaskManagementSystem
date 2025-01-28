using TaskService.Data;

namespace TaskService.Repositories.TaskSchedules;

public class TaskScheduleRepository : GenericRepository<TaskSchedule>, ITaskScheduleRepository
{
    private readonly TaskDbContext _context;

    public TaskScheduleRepository(TaskDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task RemoveRange(IEnumerable<TaskSchedule> schedules)
    {
        _context.TaskSchedules.RemoveRange(schedules);
        await _context.SaveChangesAsync();
    }
}