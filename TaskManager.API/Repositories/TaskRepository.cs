using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.DTOs;
using TaskManager.API.Interfaces;
using TaskManager.API.Models;

namespace TaskManager.API.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _appDbContext;

        public TaskRepository(AppDbContext context)
        {
            this._appDbContext = context;
        }

        public async Task<List<TaskItem>> GetAllAsync(TaskFilterDto filter, int userId)
        {
            var query = _appDbContext.Tasks.AsQueryable();
            query = query.Where(task => task.UserId == userId);

            if (filter.Priority != null)
            {
                query = query.Where(task => task.Priority == filter.Priority);
            }

            if (filter.Title != null)
            {
                query = query.Where(task => task.Title.Contains(filter.Title));
            }

            if (filter.IsCompleted != null)
            {
                query = query.Where(task =>
                    task.IsCompleted == filter.IsCompleted.Value);
            }

            switch (filter.SortBy)
            {
                case "priority":
                    if (filter.Descending)
                    {
                        query = query.OrderByDescending(task => task.Priority);
                    }
                    else
                    {
                        query = query.OrderBy(task => task.Priority);
                    }
                    break;

                case "dueDate":
                    if (filter.Descending)
                    {
                        query = query.OrderByDescending(task => task.DueDate);
                    }
                    else
                    {
                        query = query.OrderBy(task => task.DueDate);
                    }
                    break;

                default:
                    query = query.OrderBy(task => task.Id);
                    break;
            }

            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);

            return await query.ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id, int userId)
        {
            return await _appDbContext.Tasks
     .FirstOrDefaultAsync(task =>
         task.Id == id &&
         task.UserId == userId);
        }
        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            _appDbContext.Tasks.Add(task);
            await _appDbContext.SaveChangesAsync();

            return task;

        }
        public async Task<TaskItem?> UpdateAsync(TaskItem task)
        {
            var existingTask = await _appDbContext.Tasks.FindAsync(task.Id);

            if (existingTask == null)
            {
                return null;
            }

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.Priority = task.Priority;
            existingTask.IsCompleted = task.IsCompleted;

            await _appDbContext.SaveChangesAsync();

            return existingTask;
        }
        public async Task<TaskItem> DeleteAsync(int id)
        {
            var task = await _appDbContext.Tasks
           .FirstOrDefaultAsync(x=> x.Id == id);

            if (task == null)
            {
                return null;
            }

            _appDbContext.Tasks.Remove(task);

            await _appDbContext.SaveChangesAsync();

            return task;
        }
    }
}

