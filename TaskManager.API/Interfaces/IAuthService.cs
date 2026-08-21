using TaskManager.API.DTOs;
using TaskManager.API.Models;

namespace TaskManager.API.Interfaces
{  
        public interface IAuthService
        {
        Task<UserDto> RegisterAsync(RegisterDto dto);

        Task<string> LoginAsync(LoginDto dto);
        }
    }