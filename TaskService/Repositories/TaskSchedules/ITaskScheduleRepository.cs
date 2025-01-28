using TaskService.Data;

namespace TaskService.Repositories.TaskSchedules;

public interface ITaskScheduleRepository : IGenericRepository<TaskSchedule>
{
    Task RemoveRange(IEnumerable<TaskSchedule> schedules);
}