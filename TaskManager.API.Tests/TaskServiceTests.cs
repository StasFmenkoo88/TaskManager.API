using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.API.DTOs;
using TaskManager.API.Exceptions;
using TaskManager.API.Interfaces;
using TaskManager.API.Models;
using TaskManager.API.Services;

namespace TaskManager.API.Tests
{
    public class TaskServiceTests
    {
        [Fact]
        public async Task GetByIdAsync_TaskNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var repositoryMock = new Mock<ITaskRepository>();
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<TaskService>>();

            repositoryMock
                .Setup(repo => repo.GetByIdAsync(1, 1))
                .ReturnsAsync((TaskManager.API.Models.TaskItem?)null);

            var service = new TaskService(
                repositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object
            );

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.GetByIdAsync(1, 1)


            );

        }
        [Fact]
        public async Task GetByIdAsync_TaskExists_ReturnsTaskDto()
        {
            // Arrange
            var repositoryMock = new Mock<ITaskRepository>();
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<TaskService>>();

            var task = new TaskManager.API.Models.TaskItem
            {
                Id = 1,
                Title = "Learn Unit Tests"
            };

            var taskDto = new TaskDto
            {
                Id = 1,
                Title = "Learn Unit Tests"
            };

            repositoryMock
                .Setup(repo => repo.GetByIdAsync(1, 1))
                .ReturnsAsync(task);

            mapperMock
                .Setup(mapper => mapper.Map<TaskDto>(task))
                .Returns(taskDto);

            var service = new TaskService(
                repositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object
            );

            // Act
            var result = await service.GetByIdAsync(1, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Learn Unit Tests", result.Title);
        }
        [Fact]
        public async Task CreateAsync_ValidDto_CreatesTaskForUser()
        {
            // Arrange
            var repositoryMock = new Mock<ITaskRepository>();
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<TaskService>>();

            var dto = new CreateTaskDto
            {
                Title = "Gym",
                Description = "Training",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = "Medium"
            };

            var taskItem = new TaskManager.API.Models.TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Priority = dto.Priority
            };

            mapperMock
                .Setup(mapper => mapper.Map<TaskManager.API.Models.TaskItem>(dto))
                .Returns(taskItem);

            repositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<TaskManager.API.Models.TaskItem>()))
                .ReturnsAsync((TaskManager.API.Models.TaskItem task) => task);

            mapperMock
                .Setup(mapper => mapper.Map<TaskDto>(It.IsAny<TaskManager.API.Models.TaskItem>()))
                .Returns((TaskManager.API.Models.TaskItem task) => new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title
                });

            var service = new TaskService(
                repositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object
            );

            int userId = 5;

            // Act
            var result = await service.CreateAsync(dto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Gym", result.Title);

            Assert.Equal(5, taskItem.UserId);
            Assert.False(taskItem.IsCompleted);

            repositoryMock.Verify(
    repo => repo.CreateAsync(It.IsAny<TaskItem>()),
    Times.Once
);
        }
        [Fact]
        public async Task UpdateAsync_TaskExists_UpdatesTask()
        {
            // Arrange
            var repositoryMock = new Mock<ITaskRepository>();
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<TaskService>>();

            var existingTask = new TaskItem
            {
                Id = 1,
                Title = "Old title",
                Description = "Old description",
                Priority = "Low",
                UserId = 5
            };

            var dto = new UpdateTaskDto
            {
                Title = "New title",
                Description = "New description",
                Priority = "High"
            };

            repositoryMock
                .Setup(repo => repo.GetByIdAsync(1, 5))
                .ReturnsAsync(existingTask);

            mapperMock
                .Setup(mapper => mapper.Map(dto, existingTask))
                .Callback(() =>
                {
                    existingTask.Title = dto.Title;
                    existingTask.Description = dto.Description;
                    existingTask.Priority = dto.Priority;
                });

            repositoryMock
                .Setup(repo => repo.UpdateAsync(existingTask))
                .ReturnsAsync(existingTask);

            mapperMock
                .Setup(mapper => mapper.Map<TaskDto>(existingTask))
                .Returns(new TaskDto
                {
                    Id = existingTask.Id,
                    Title = "New title",
                    Description = "New description",
                    Priority = "High"
                });

            var service = new TaskService(
                repositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object
            );

            // Act
            var result = await service.UpdateAsync(1, dto, 5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New title", result.Title);
            Assert.Equal("High", result.Priority);

            repositoryMock.Verify(
                repo => repo.UpdateAsync(existingTask),
                Times.Once
            );
        }
        [Fact]
        public async Task UpdateAsync_TaskNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var repositoryMock = new Mock<ITaskRepository>();
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<TaskService>>();

            var dto = new UpdateTaskDto
            {
                Title = "New title",
                Description = "New description",
                Priority = "High"
            };

            repositoryMock
                .Setup(repo => repo.GetByIdAsync(1, 5))
                .ReturnsAsync((TaskItem?)null);

            var service = new TaskService(
                repositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object
            );

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.UpdateAsync(1, dto, 5)
            );

            repositoryMock.Verify(
                repo => repo.UpdateAsync(It.IsAny<TaskItem>()),
                Times.Never
            );
        }
        [Fact]
        public async Task DeleteAsync_TaskExists_DeletesTask()
        {
            // Arrange
            var repositoryMock = new Mock<ITaskRepository>();
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<TaskService>>();

            var task = new TaskItem
            {
                Id = 1,
                UserId = 5,
                Title = "Delete me"
            };
            repositoryMock
     .Setup(repo => repo.GetByIdAsync(1, 5))
     .ReturnsAsync(task);

            var service = new TaskService(
                repositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object
                );

            var result = await service.DeleteAsync(1, 5);

            Assert.True(result);

            repositoryMock.Verify(
     repo => repo.DeleteAsync(1),
     Times.Once
 );

            // продолжи сам

        }
        [Fact]
        public async Task DeleteAsync_TaskNotFound_ThrowsNotFoundException()
        {
             var repositoryMock = new Mock<ITaskRepository>();
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<TaskService>>();

         
            repositoryMock
                .Setup(repo => repo.GetByIdAsync(1, 5))
                .ReturnsAsync((TaskItem?)null);

            var service = new TaskService(
                repositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object
            );

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.DeleteAsync(1, 5)
            );

            repositoryMock.Verify(
                repo => repo.DeleteAsync(1),
                Times.Never
            );  // Arrange
        }
        [Fact]
        public async Task GetAllAsync_TasksExist_ReturnsTasks()
        {
            // Arrange
            var repositoryMock = new Mock<ITaskRepository>();
            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<TaskService>>();

            var filter = new TaskFilterDto();

            var tasks = new List<TaskItem>
    {
        new TaskItem
        {
            Id = 1,
            Title = "Gym",
            UserId = 5
        },
        new TaskItem
        {
            Id = 2,
            Title = "Study C#",
            UserId = 5
        }
    };

            var taskDtos = new List<TaskDto>
    {
        new TaskDto
        {
            Id = 1,
            Title = "Gym"
        },
        new TaskDto
        {
            Id = 2,
            Title = "Study C#"
        }
    };

            repositoryMock
                .Setup(repo => repo.GetAllAsync(filter, 5))
                .ReturnsAsync(tasks);

            mapperMock
                .Setup(mapper => mapper.Map<List<TaskDto>>(tasks))
                .Returns(taskDtos);

            var service = new TaskService(
                repositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object
            );

            // Act
            var result = await service.GetAllAsync(filter, 5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Equal("Gym", result[0].Title);
            Assert.Equal("Study C#", result[1].Title);

            repositoryMock.Verify(
                repo => repo.GetAllAsync(filter, 5),
                Times.Once
            );
        }
    }
}