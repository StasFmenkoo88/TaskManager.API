using TaskManager.API.DTOs;

namespace TaskManager.API.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetAllAsync(TaskFilterDto filter, int userId);

        Task<TaskDto?> GetByIdAsync(int id, int userId);

        Task<TaskDto> CreateAsync(CreateTaskDto dto, int userId);

        Task<TaskDto?> UpdateAsync(int id, UpdateTaskDto dto, int userId);
        Task<bool> DeleteAsync(int id,int userId);
    }
}
