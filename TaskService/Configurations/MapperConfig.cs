using AutoMapper;
using TaskService.Data;
using TaskService.Models.TaskCompletions;
using TaskService.Models.Tasks;
using TaskService.Models.Topics;

namespace TaskService.Configurations;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<TaskCreateDto, TaskModel>().ReverseMap();
        CreateMap<TaskReadOnlyDto, TaskModel>().ReverseMap();
        CreateMap<TaskUpdateDto, TaskModel>().ReverseMap();

        CreateMap<TopicCreateDto, TopicModel>().ReverseMap();
        CreateMap<TopicReadOnlyDto, TopicModel>().ReverseMap();
        CreateMap<TopicUpdateDto, TopicModel>().ReverseMap();

        CreateMap<TaskCompletionCreateDto, TaskCompletion>().ReverseMap();
    }
}