using System.ComponentModel.DataAnnotations;
using TaskService.Data;

namespace TaskService.Models.TaskCompletions;

public class TaskCompletionCreateDto : BaseDto
{
    [Required]
    public int TaskId { get; set; }
    
    public TaskModel? Task { get; set; }
    [Required]
    public DateTime? CompletionDate { get; set; }
}