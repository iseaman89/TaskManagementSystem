using System.ComponentModel.DataAnnotations;
using TaskService.Data;

namespace TaskService.Models.Topics;

public class TopicUpdateDto : BaseDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Color { get; set; }

    public string[] Tags { get; set; }
    
    public List<TaskModel>? TaskCompletions { get; set; }
    
    [Required]
    public string AssignedUserId { get; set; }
}