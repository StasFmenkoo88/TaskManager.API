using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.DTOs;
using TaskManager.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {

        private readonly ITaskService _taskService;

        private readonly IValidator<CreateTaskDto> _createTaskValidator;

        private readonly IValidator<UpdateTaskDto> _updateTaskValidator;
        public TaskController(
     ITaskService taskService,
     IValidator<CreateTaskDto> createTaskValidator,
     IValidator<UpdateTaskDto> updateTaskValidator)
        {
            _taskService = taskService;
            _createTaskValidator = createTaskValidator;
            _updateTaskValidator = updateTaskValidator;
        }
        [HttpGet]
        public async Task<ActionResult<List<TaskDto>>> GetAll([FromQuery] TaskFilterDto filter)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var tasks = await _taskService.GetAllAsync(filter, userId);

            return Ok(tasks);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetById(int id)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var task = await _taskService.GetByIdAsync(id, userId);

            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> Create(CreateTaskDto dto)
        {
            var validationResult = await _createTaskValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var task = await _taskService.CreateAsync(dto, userId);

            return Ok(task);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<TaskDto>> Update(
      int id,
      UpdateTaskDto dto)
        {
            var validationResult =
                await _updateTaskValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var userId = int.Parse(
     User.FindFirst(ClaimTypes.NameIdentifier)!.Value
 );

            var task = await _taskService.UpdateAsync(id, dto, userId);

            return Ok(task);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAsync(int id)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var result = await _taskService.DeleteAsync(id, userId);

            if (!result)
            {
                return NotFound();
            }

            return Ok(true);
        }
    }

    }
