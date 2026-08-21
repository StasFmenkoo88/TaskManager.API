using Microsoft.AspNetCore.Mvc;
using TaskManager.API.DTOs;
using TaskManager.API.Models;
namespace TaskManager.API.Interfaces;
   public interface ITaskRepository
    {
    Task<List<TaskItem>> GetAllAsync(TaskFilterDto filter, int userId);

    Task<TaskItem?> GetByIdAsync(int id, int userId);

    Task<TaskItem> CreateAsync(TaskItem task);

    Task<TaskItem?> UpdateAsync(TaskItem task);

    Task<TaskItem> DeleteAsync(int id);
}
