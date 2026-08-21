using TaskManager.API.Models;

namespace TaskManager.API.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User> CreateAsync(User user);

        Task<bool> EmailExistsAsync(string email);
    }
}