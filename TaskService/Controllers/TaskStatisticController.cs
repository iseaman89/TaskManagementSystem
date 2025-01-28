using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagmentSystem.Data.Shared.Statics;
using TaskService.Data;
using TaskService.Models.Statistics;
using TaskService.Repositories.TaskStatistics;

namespace TaskService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TaskStatisticController : ControllerBase
{
    private readonly ITaskStatisticRepository _statisticRepository;
    private readonly ILogger<TaskStatisticController> _logger;

    public TaskStatisticController(ITaskStatisticRepository statisticRepository, ILogger<TaskStatisticController> logger)
    {
        _statisticRepository = statisticRepository;
        _logger = logger;
    }
    
    [HttpGet("today/{userId}")]
    public async Task<IActionResult> GetTodayStatistics(string userId)
    {
        return await GetStatisticsForPeriod(userId, DateTime.Today, DateTime.Today.AddDays(1), "today");
    }

    [HttpGet("week/{userId}")]
    public async Task<IActionResult> GetWeekStatistics(string userId)
    {
        var startOfWeek = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek + 6) % 7);
        var endOfWeek = startOfWeek.AddDays(7);
        return await GetStatisticsForPeriod(userId, startOfWeek, endOfWeek, "week");
    }

    [HttpGet("month/{userId}")]
    public async Task<IActionResult> GetMonthStatistics(string userId)
    {
        var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);
        return await GetStatisticsForPeriod(userId, startOfMonth, endOfMonth, "month");
    }

    [HttpGet("year/{userId}")]
    public async Task<IActionResult> GetYearStatistics(string userId)
    {
        var startOfYear = new DateTime(DateTime.Today.Year, 1, 1);
        var endOfYear = startOfYear.AddYears(1);
        return await GetStatisticsForPeriod(userId, startOfYear, endOfYear, "year");
    }
    
    private async Task<IActionResult> GetStatisticsForPeriod(string userId, DateTime startDate, DateTime endDate, string periodName)
    {
        try
        {
            var topics = await _statisticRepository.GetTopicsByUserId(userId);
            var topicStatistics = new List<TopicStatistic>();

            foreach (var topic in topics)
            {
                var completedTasks = 0;
                var schedules = 0;
                var taskStatistics = new List<TaskStatistic>();

                foreach (var task in topic.Tasks)
                {
                    // Рахуємо кількість завершених завдань для поточного періоду
                    var completed = task.TaskCompletions.Count(tc => tc.CompletionDate >= startDate && tc.CompletionDate < endDate);

                    // Рахуємо кількість запланованих завдань для періоду
                    var scheduled = CalculateSchedulesForPeriod(task.TaskSchedules, startDate, endDate);

                    completedTasks += completed;
                    schedules += scheduled;

                    var completionRate = CompletionRate.CalculateCompletionRate(completed, scheduled);

                    taskStatistics.Add(new TaskStatistic
                    {
                        TaskName = task.Title,
                        CompletedTasks = completed,
                        Schedules = scheduled,
                        CompletionRate = completionRate
                    });
                }

                var topicCompletionRate = CompletionRate.CalculateCompletionRate(completedTasks, schedules);

                topicStatistics.Add(new TopicStatistic
                {
                    TopicTitel = topic.Name,
                    Color = topic.Color,
                    CompletedTasks = completedTasks,
                    Schedules = schedules,
                    CompletionRate = topicCompletionRate,
                    TaskStatistics = taskStatistics
                });
            }

            return Ok(topicStatistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error Performing GET in {nameof(GetStatisticsForPeriod)} for {periodName}");
            return StatusCode(500, Messages.Error500Message);
        }
    }
    
    private int CalculateSchedulesForPeriod(IEnumerable<TaskSchedule> taskSchedules, DateTime startDate, DateTime endDate)
    {
        int totalSchedules = 0;

        foreach (var schedule in taskSchedules)
        {
            // Для кожного дня між startDate і endDate перевіряємо, чи відповідає цей день дню тижня, вказаному у розкладі
            for (var date = startDate; date < endDate; date = date.AddDays(1))
            {
                if (schedule.DayOfWeek == date.DayOfWeek)
                {
                    totalSchedules++;
                }
            }
        }

        return totalSchedules;
    }

    /*[HttpGet("today/{userId}")]
    public async Task<IActionResult> GetTodayStatistics(string userId)
    {
        try
        {
            var topics = await _statisticRepository.GetTopicsByUserId(userId);
            var today = DateTime.Today;
            var topicStatistics = new List<TopicStatistic>();
            
            foreach (var topic in topics)
            {
                var completedTasks = 0;
                var schedules = 0;

                var taskStatistics = new List<TaskStatistic>();

                foreach (var task in topic.Tasks)
                {
                    var completed = task.TaskCompletions.Count(tc => tc.CompletionDate == today);
                    var scheduled = task.TaskSchedules.Count(ts => ts.DayOfWeek == today.DayOfWeek);

                    completedTasks += completed;
                    schedules += scheduled;

                    var completionRate = CompletionRate.CalculateCompletionRate(completed, scheduled);

                    taskStatistics.Add(new TaskStatistic
                    {
                        TaskName = task.Title,
                        CompletedTasks = completed,
                        Schedules = scheduled,
                        CompletionRate = completionRate
                    });
                }

                var topicCompletionRate = CompletionRate.CalculateCompletionRate(completedTasks, schedules);

                topicStatistics.Add(new TopicStatistic
                {
                    TopicTitel = topic.Name,
                    Color = topic.Color,
                    CompletedTasks = completedTasks,
                    Schedules = schedules,
                    CompletionRate = topicCompletionRate,
                    TaskStatistics = taskStatistics 
                });
            }

            return Ok(topicStatistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error Performing GET in {nameof(GetTodayStatistics)}");
            return StatusCode(500, Messages.Error500Message);
        }
    }

    [HttpGet("week/{userId}")]
    public async Task<IActionResult> GetWeekStatistics(string userId)
    {
        try
        {
            var topics = await _statisticRepository.GetTopicsByUserId(userId);
            var topicStatistics = new List<TopicStatistic>();

            var startOfWeek = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek + 6) % 7);
            var endOfWeek = startOfWeek.AddDays(7);

            foreach (var topic in topics)
            {
                var completedTasks = 0;
                var schedules = 0;

                var taskStatistics = new List<TaskStatistic>();

                foreach (var task in topic.Tasks)
                {
                    var completed = task.TaskCompletions.Count(tc => tc.CompletionDate >= startOfWeek && tc.CompletionDate < endOfWeek);
                    var scheduled = task.TaskSchedules.Count();

                    completedTasks += completed;
                    schedules += scheduled;

                    var completionRate = CompletionRate.CalculateCompletionRate(completed, scheduled);

                    taskStatistics.Add(new TaskStatistic
                    {
                        TaskName = task.Title,
                        CompletedTasks = completed,
                        Schedules = scheduled,
                        CompletionRate = completionRate
                    });
                }

                var topicCompletionRate = CompletionRate.CalculateCompletionRate(completedTasks, schedules);

                topicStatistics.Add(new TopicStatistic
                {
                    TopicTitel = topic.Name,
                    Color = topic.Color,
                    CompletedTasks = completedTasks,
                    Schedules = schedules,
                    CompletionRate = topicCompletionRate,
                    TaskStatistics = taskStatistics 
                });
            }

            return Ok(topicStatistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error Performing GET in {nameof(GetWeekStatistics)}");
            return StatusCode(500, Messages.Error500Message);
        }
    }

    [HttpGet("month/{userId}")]
    public async Task<IActionResult> GetMonthStatistics(string userId)
    {
        try
        {
            var topics = await _statisticRepository.GetTopicsByUserId(userId);
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);
            var topicStatistics = new List<TopicStatistic>();
            
            foreach (var topic in topics)
            {
                var completedTasks = 0;
                var schedules = 0;

                var taskStatistics = new List<TaskStatistic>();

                foreach (var task in topic.Tasks)
                {
                    var completed = task.TaskCompletions.Count(tc => tc.CompletionDate >= startOfMonth && tc.CompletionDate < endOfMonth);
                    var scheduled = task.TaskSchedules.Sum(ts => DaysOfWeekCounter.CountDaysOfWeekInMonth(ts.DayOfWeek));

                    completedTasks += completed;
                    schedules += scheduled;

                    var completionRate = CompletionRate.CalculateCompletionRate(completed, scheduled);

                    taskStatistics.Add(new TaskStatistic
                    {
                        TaskName = task.Title,
                        CompletedTasks = completed,
                        Schedules = scheduled,
                        CompletionRate = completionRate
                    });
                }

                var topicCompletionRate = CompletionRate.CalculateCompletionRate(completedTasks, schedules);

                topicStatistics.Add(new TopicStatistic
                {
                    TopicTitel = topic.Name,
                    Color = topic.Color,
                    CompletedTasks = completedTasks,
                    Schedules = schedules,
                    CompletionRate = topicCompletionRate,
                    TaskStatistics = taskStatistics 
                });
            }

            return Ok(topicStatistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error Performing GET in {nameof(GetMonthStatistics)}");
            return StatusCode(500, Messages.Error500Message);
        }
    }
    
    [HttpGet("year/{userId}")]
    public async Task<IActionResult> GetYearStatistics(string userId)
    {
        try
        {
            var topics = await _statisticRepository.GetTopicsByUserId(userId);
            var startOfYear = new DateTime(DateTime.Today.Year, 1, 1);
            var endOfYear = startOfYear.AddYears(1);
            var topicStatistics = new List<TopicStatistic>();
            
            foreach (var topic in topics)
            {
                var completedTasks = 0;
                var schedules = 0;

                var taskStatistics = new List<TaskStatistic>();

                foreach (var task in topic.Tasks)
                {
                    var completed = task.TaskCompletions.Count(tc => tc.CompletionDate >= startOfYear && tc.CompletionDate < endOfYear);
                    var scheduled = task.TaskSchedules.Sum(ts => DaysOfWeekCounter.CountDaysOfWeekInYear(ts.DayOfWeek));

                    completedTasks += completed;
                    schedules += scheduled;

                    var completionRate = CompletionRate.CalculateCompletionRate(completed, scheduled);

                    taskStatistics.Add(new TaskStatistic
                    {
                        TaskName = task.Title,
                        CompletedTasks = completed,
                        Schedules = scheduled,
                        CompletionRate = completionRate
                    });
                }

                var topicCompletionRate = CompletionRate.CalculateCompletionRate(completedTasks, schedules);

                topicStatistics.Add(new TopicStatistic
                {
                    TopicTitel = topic.Name,
                    Color = topic.Color,
                    CompletedTasks = completedTasks,
                    Schedules = schedules,
                    CompletionRate = topicCompletionRate,
                    TaskStatistics = taskStatistics 
                });
            }

            return Ok(topicStatistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error Performing GET in {nameof(GetYearStatistics)}");
            return StatusCode(500, Messages.Error500Message);
        }
    }*/
}