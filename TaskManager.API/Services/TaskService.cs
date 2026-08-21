using TaskManager.API.DTOs;
using TaskManager.API.Interfaces;
using TaskManager.API.Models;
using TaskManager.API.Repositories;
using AutoMapper;
using System.Threading.Tasks;
using TaskManager.API.Exceptions;


namespace TaskManager.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly IMapper _mapper;

        private readonly TaskRepository _taskRepository;

        private readonly ILogger<TaskService> _logger;

        public TaskService(TaskRepository taskRepository, IMapper mapper,ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TaskDto?> GetByIdAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id, userId);

            if (task == null)
                throw new NotFoundException("Task not found"); 

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<List<TaskDto>> GetAllAsync(TaskFilterDto filter, int userId)
        {
            var tasks = await _taskRepository.GetAllAsync(filter, userId);

            return _mapper.Map<List<TaskDto>>(tasks);
        }

        public async Task<TaskDto> CreateAsync(CreateTaskDto dto, int userId)
        {
            _logger.LogInformation("Creating task: {Title}", dto.Title);

            var task = _mapper.Map<TaskItem>(dto);    

            task.IsCompleted = false;
            task.CreatedAt = DateTime.UtcNow;
            task.UserId = userId;

            var createdTask = await _taskRepository.CreateAsync(task);
            _logger.LogInformation(
    "Task created successfully. Id: {Id}",
    createdTask.Id);


            return _mapper.Map<TaskDto>(createdTask);
        }


        public async Task<TaskDto?> UpdateAsync(int id, UpdateTaskDto dto, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id,userId);

            if (task == null)
            {
                throw new NotFoundException("Task not found");
            }

            _mapper.Map(dto, task);

            var updatedTask = await _taskRepository.UpdateAsync(task);

            return _mapper.Map<TaskDto>(updatedTask);
        }
        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id, userId);

            if (task == null)
            {
                throw new NotFoundException("Task not found");
            }

            await _taskRepository.DeleteAsync(id);

            return true;
        }
    }
}