using System.ComponentModel.DataAnnotations;
using TaskManagmentSystem.Data.Shared.Enums;
using TaskService.Data;

namespace TaskService.Models.Tasks;

public class TaskCreateDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [Required]
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public List<TaskSchedule>? TaskSchedules { get; set; }
    
    public List<TaskCompletion>? TaskCompletions { get; set; }
    
    [Required]
    public int TopicId { get; set; }
    
    [Required]
    public string AssignedUserId { get; set; }
}