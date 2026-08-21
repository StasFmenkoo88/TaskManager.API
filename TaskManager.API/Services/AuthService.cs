using Microsoft.AspNetCore.Identity;
using TaskManager.API.DTOs;
using TaskManager.API.Interfaces;
using TaskManager.API.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
namespace TaskManager.API.Services;
    using TaskManager.API.Exceptions;

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        private readonly PasswordHasher<User> _passwordHasher;

        private readonly IConfiguration _configuration;

        public AuthService(
    IUserRepository userRepository,
    PasswordHasher<User> passwordHasher,
    IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }
        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            var emailExist = await _userRepository.EmailExistsAsync(dto.Email);

            if (emailExist)
            {
                throw new ConflictException("Email already exists");
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email
            };

            user.PasswordHash =
     _passwordHasher.HashPassword(user, dto.Password);

            var createdUser = await _userRepository.CreateAsync(user);

            return new UserDto
            {
                Id = createdUser.Id,
                Name = createdUser.Name,
                Email = createdUser.Email
            };
        }
        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null)
            {
            throw new UnauthorizedException("Invalid email or password");
        }
            var result = _passwordHasher.VerifyHashedPassword(
    user,
    user.PasswordHash,
    dto.Password
);
            if (result == PasswordVerificationResult.Failed)
            {
            throw new UnauthorizedException("Invalid email or password");
        }
            var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Email, user.Email)
};

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );
            var credentials = new SigningCredentials(
    key,
    SecurityAlgorithms.HmacSha256
);
            var token = new JwtSecurityToken(
    issuer: _configuration["Jwt:Issuer"],
    audience: null,
    claims: claims,
    expires: DateTime.UtcNow.AddHours(1),
    signingCredentials: credentials
);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    

