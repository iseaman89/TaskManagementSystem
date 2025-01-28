using Microsoft.EntityFrameworkCore;

namespace TaskService.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }
    
    public DbSet<TaskModel?> Tasks { get; set; }
    public DbSet<TopicModel?> Topics { get; set; }
    public DbSet<TaskSchedule> TaskSchedules { get; set; }
    public DbSet<TaskCompletion> TaskCompletions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskModel>()
            .HasMany(t => t.TaskSchedules)
            .WithOne(ts => ts.Task)
            .HasForeignKey(ts => ts.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<TaskModel>()
            .HasMany(t => t.TaskCompletions)
            .WithOne(tc => tc.Task)
            .HasForeignKey(tc => tc.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TopicModel>()
            .HasMany(t => t.Tasks)
            .WithOne(t => t.Topic)
            .HasForeignKey(t => t.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}