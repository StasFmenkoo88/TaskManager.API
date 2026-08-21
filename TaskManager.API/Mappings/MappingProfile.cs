using AutoMapper;
using TaskManager.API.DTOs;
using TaskManager.API.Models;

namespace TaskManager.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaskItem, TaskDto>();

            CreateMap<CreateTaskDto, TaskItem>();

            CreateMap<UpdateTaskDto, TaskItem>();
        }
    }
}