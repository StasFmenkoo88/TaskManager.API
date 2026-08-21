using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.Interfaces;
using TaskManager.API.Models;

namespace TaskManager.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _appDbContext.Users
     .FirstOrDefaultAsync(user => user.Email == email);
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _appDbContext.Users
     .AnyAsync(user => user.Email == email);
        }
        public async Task<User> CreateAsync(User user)
        {
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

            return user;

        }
    }
}
