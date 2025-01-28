using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagmentSystem.Data.Shared.Statics;
using TaskService.Data;
using TaskService.Models.TaskCompletions;
using TaskService.Models.Tasks;
using TaskService.Repositories.TaskCompletions;
using TaskService.Repositories.Tasks;
using TaskService.Repositories.TaskSchedules;

namespace TaskService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskScheduleRepository _scheduleRepository;
        private readonly ITaskCompletionRepository _completionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TaskController> _logger;

        public TaskController(ITaskRepository taskRepository, ITaskScheduleRepository scheduleRepository, ITaskCompletionRepository completionRepository, IMapper mapper, ILogger<TaskController> logger)
        {
            _taskRepository = taskRepository;
            _scheduleRepository = scheduleRepository;
            _completionRepository = completionRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TaskReadOnlyDto>>> GetTasks(string userId)
        {
            try
            {
                var tasks = await _taskRepository.GetTasksByUserId(userId);
                var taskDtos = _mapper.Map<IEnumerable<TaskReadOnlyDto>>(tasks);
                return Ok(taskDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing GET in {nameof(GetTasks)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        [HttpGet("date/{userId}")]
        public async Task<ActionResult<IEnumerable<TaskForToday>>> GetTasksByDate(string userId)
        {
            try
            {
                var tasksForToday = new List<TaskForToday>();
                var tasks = await _taskRepository.GetTasksByDate(userId);
                foreach (var task in tasks)
                {
                    var color = _taskRepository.GetTopicForColor(task.TopicId).Result?.Color;
                    tasksForToday.Add(new TaskForToday
                    {
                        Color = color,
                        Id = task.Id,
                        Priority = task.Priority,
                        Title = task.Title
                    });
                }
                return Ok(tasksForToday);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing GET in {nameof(GetTasksByDate)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskReadOnlyDto>> GetTask(int id)
        {
            try
            {
                var task = await _taskRepository.GetAsync(id);

                if (task == null)
                {
                    _logger.LogWarning($"Record Not Found: {nameof(GetTask)} - ID: {id}");
                    return NotFound();
                }

                var taskDto = _mapper.Map<TaskReadOnlyDto>(task);
                return Ok(taskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing GET in {nameof(GetTasks)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutTask(int id, TaskUpdateDto taskDto)
        {
            if (id != taskDto.Id)
            {
                _logger.LogWarning($"Update ID invalid in {nameof(PutTask)} - ID: {id}");
                return BadRequest();
            }
            
            var task = await _taskRepository.GetTaskByUserId(id);

            if (task == null)
            {
                _logger.LogWarning($"{nameof(Task)} record not found in {nameof(PutTask)} - ID: {id}");
                return NotFound();
            }

            if (task.TaskSchedules != null && task.TaskSchedules.Any())
                await _scheduleRepository.RemoveRange(task.TaskSchedules);

            _mapper.Map(taskDto, task);

            try
            {
                await _taskRepository.UpdateAsync(task);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Error Performing Put in {nameof(PutTask)}");
                    return StatusCode(500, Messages.Error500Message);
                }
            }

            return NoContent();
        }
        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TaskCreateDto>> PostTask(TaskCreateDto taskDto)
        {
            try
            {
                var task = _mapper.Map<TaskModel>(taskDto);
                await _taskRepository.AddAsync(task);
                
                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing POST in {nameof(PostTask)} Schedule count: {{{{task?.TaskSchedules?.Count}}}}", taskDto);
                return StatusCode(500, Messages.Error500Message);
            }
            
        }
        
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var task = await _taskRepository.GetAsync(id);
                if (task == null)
                {
                    _logger.LogWarning($"{nameof(Task)} record not found in {nameof(DeleteTask)} - ID: {id}");
                    return NotFound();
                }

                await _taskRepository.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing DELETE in {nameof(DeleteTask)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        [HttpDelete("task-completion/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTaskCompletion(int id)
        {
            try
            {
                var completion = await _completionRepository.GetAsync(id);
                if (completion == null)
                {
                    _logger.LogWarning($"{nameof(Task)} record not found in {nameof(DeleteTaskCompletion)} - ID: {id}");
                    return NotFound();
                }

                await _completionRepository.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing DELETE in {nameof(DeleteTask)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        [HttpPost("completion")]
        [Authorize]
        public async Task<IActionResult> PostTaskCompletion(TaskCompletionCreateDto completionDto)
        {
            try
            {
                var completion = _mapper.Map<TaskCompletion>(completionDto);
                await _completionRepository.AddAsync(completion);
                
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing POST in {nameof(PostTask)} Schedule count: {{{{task?.TaskSchedules?.Count}}}}", completionDto);
                return StatusCode(500, Messages.Error500Message);
            }
        }

        private async Task<bool> TaskExists(int id)
        {
            return await _taskRepository.Exists(id);
        }
    }
}
