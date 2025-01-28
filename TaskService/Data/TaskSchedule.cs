using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskService.Data;

public class TaskSchedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int TaskId { get; set; }
    public TaskModel? Task { get; set; }
    public string? Options { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
}