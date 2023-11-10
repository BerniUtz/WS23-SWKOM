using Microsoft.AspNetCore.Mvc;
using FizzWare.NBuilder;
using AutoMapper;
using Mock_Server.Models;

namespace Mock_Server.Controllers;

[ApiController]
[Route("/api/tags/")]
// [Route("[controller]")]
public class TagsController : ControllerBase
{
    private ILogger<TagsController> _logger;
    private readonly IMapper _mapper;

    public TagsController(IMapper mapper, ILogger<TagsController> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet(Name = "GetTags")]
    public IActionResult GetTags()
    {
        Random r = new Random();
        int count = 4;
        return this.Ok(new ListResponse<Tag>()
        {
            Count = count,
            Next = null,
            Previous = null,
            Results = Builder<Tag>.CreateListOfSize(count)
                .All()
                    .With(p => p.Color = RandomColorGenerator.GetRandomColor().ToHex())
                    .With(p => p.MatchingAlgorithm = r.Next(1, 6))
                .Build()
        });
    }

    [HttpPost(Name = "CreateTag")]
    public IActionResult CreateTag([FromBody] NewTag newTag)
    {
        var tag = _mapper.Map<Tag>(newTag);

        tag.Id = 1;
        tag.Slug = "foo";

        return Created($"/api/tags/{tag.Id}", tag);
    }

    [HttpPut("{id:int}", Name = "UpdateTag")]
    public IActionResult UpdateTag([FromRoute] int id, [FromBody] Tag tag)
    {
        return Ok(tag);
    }

    [HttpDelete("{id:int}", Name = "DeleteTag")]
    public IActionResult DeleteTag([FromRoute] int id)
    {
        return NoContent();
    }
}
