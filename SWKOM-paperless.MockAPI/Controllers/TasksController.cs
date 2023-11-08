using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Mock_Server.Models;

namespace Mock_Server.Controllers;

[ApiController]
[Route("/api")]
public class TasksController : ControllerBase
{
    private readonly ILogger<TasksController> _logger;

    public TasksController(ILogger<TasksController> logger)
    {
        _logger = logger;
    }

    [HttpGet("tasks")]
    public IActionResult GetTasks()
    {
        Random rand = new Random();
        int count = 10;

        return Ok(Builder<Models.Task>.CreateListOfSize(count)
                        .All()
                        .With(p => p.Type = Models.TaskType.file)
                        .With(p => p.DateCreated = DateTimeOffset.Now.AddDays(rand.Next(-10, 0)))
                        .With(p => p.DateDone = DateTimeOffset.Now.AddDays(rand.Next(-10, 0)))
                        .Build());
    }

    [HttpPost("acknowledge_tasks")]
    public IActionResult AckTasks([FromBody] AckTasksRequest tasks)
    {
        _logger.LogInformation($"Acknowledge tasks: {string.Join(", ", tasks.Tasks)}");
        return Ok();
    }
}

