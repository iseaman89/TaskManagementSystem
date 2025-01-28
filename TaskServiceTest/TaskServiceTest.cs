using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagmentSystem.Data.Shared.Enums;
using TaskService;
using TaskService.Controllers;
using TaskService.Data;
using TaskService.Models;
using TaskService.Models.Tasks;
using TaskService.Repositories;
using TaskService.Repositories.Tasks;
using TaskService.Repositories.TaskSchedules;
using TaskServiceTest;
using Xunit;

public class TaskControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public TaskControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetTasks_ReturnsTasks()
    {
        // Arrange
        var scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();

        var mockMapper = new Mock<IMapper>();
        var mockLogger = new Mock<ILogger<TaskController>>();

        var taskDto = new TaskReadOnlyDto()
        {
            Id = 1,
            Title = "Test TaskModel",
          
        };

        var task = new TaskModel
        {
            Id = 1,
            Title = "Test TaskModel",
          
        };

        context.Tasks.Add(task);
        context.SaveChanges();

        var taskRepository = new TaskRepository(context);
        var scheduleRepository = new TaskScheduleRepository(context);
        
        mockMapper.Setup(m => m.Map<TaskReadOnlyDto>(It.IsAny<TaskModel>())).Returns(taskDto);

        var controller = new TaskController(taskRepository, scheduleRepository, mockMapper.Object, mockLogger.Object);

        // Act
        var result = await controller.GetTasks();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<TaskReadOnlyDto>>(okResult.Value);
        Assert.Single(returnValue);
        Assert.Equal("Test TaskModel", returnValue[0].Title);
    }

    [Fact]
    public async Task CreateTask_CreatesTask()
    {
        // Arrange
        var scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>();

        var mockMapper = new Mock<IMapper>();
        var mockLogger = new Mock<ILogger<TaskController>>();

        var taskDto = new TaskCreateDto()
        {
            Title = "New TaskModel",
        };

        var task = new TaskModel
        {
            Title = "New TaskModel",
        
        };

        var taskRepository = new TaskRepository(context);
        var scheduleRepository = new TaskScheduleRepository(context);
        
        mockMapper.Setup(m => m.Map<TaskCreateDto>(It.IsAny<TaskModel>())).Returns(taskDto);

        var controller = new TaskController(taskRepository, scheduleRepository, mockMapper.Object, mockLogger.Object);

        // Act
        var result = await controller.PostTask(taskDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<TaskCreateDto>(okResult.Value);
        Assert.Equal("New TaskModel", returnValue.Title);
    }
}
