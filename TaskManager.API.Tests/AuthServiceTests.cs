using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using TaskManager.API.DTOs;
using TaskManager.API.Exceptions;
using TaskManager.API.Interfaces;
using TaskManager.API.Models;
using TaskManager.API.Services;

namespace TaskManager.API.Tests
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task LoginAsync_UserNotFound_ThrowsUnauthorizedException()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();

            var passwordHasher = new PasswordHasher<User>();

            var configurationMock = new Mock<IConfiguration>();

            userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync("wrong@test.com"))
                .ReturnsAsync((User?)null);

            var service = new AuthService(
                userRepositoryMock.Object,
                passwordHasher,
                configurationMock.Object
            );

            var dto = new LoginDto
            {
                Email = "wrong@test.com",
                Password = "123456"
            };

            // Act + Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                service.LoginAsync(dto)
            );
        }
        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsJwtToken()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();

            var passwordHasher = new PasswordHasher<User>();

            var configurationData = new Dictionary<string, string?>
    {
        { "Jwt:Key", "super-secret-taskmanager-key-2026" },
        { "Jwt:Issuer", "TaskManager.API" }
    };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            var user = new User
            {
                Id = 1,
                Name = "Test",
                Email = "test@test.com"
            };

            user.PasswordHash =
                passwordHasher.HashPassword(user, "Test123!");

            userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync("test@test.com"))
                .ReturnsAsync(user);

            var service = new AuthService(
                userRepositoryMock.Object,
                passwordHasher,
                configuration
            );

            var dto = new LoginDto
            {
                Email = "test@test.com",
                Password = "Test123!"
            };

            // Act
            var token = await service.LoginAsync(dto);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            userRepositoryMock.Verify(
                repo => repo.GetByEmailAsync("test@test.com"),
                Times.Once
            );
        }
    }
}