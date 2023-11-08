using Microsoft.AspNetCore.Mvc;
using FizzWare.NBuilder;
using AutoMapper;
using Mock_Server.Models;

namespace Mock_Server.Controllers;

[ApiController]
[Route("/api/correspondents/")]
// [Route("[controller]")]
public class CorrespondentsController : ControllerBase
{
    private ILogger<CorrespondentsController> _logger;
    private readonly IMapper _mapper;

    public CorrespondentsController(IMapper mapper, ILogger<CorrespondentsController> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet(Name = "GetCorrespondents")]
    public IActionResult GetCorrespondents()
    {
        Random r = new Random();
        int count = 7;
        return this.Ok(new ListResponse<Correspondent>()
        {
            Count = count,
            Next = null,
            Previous = null,
            Results = Builder<Correspondent>.CreateListOfSize(count)
                .All()
                .With(p => p.MatchingAlgorithm = r.Next(1, 6))
                .Build()
        });
    }

    [HttpPost(Name = "CreateCorrespondent")]
    public IActionResult CreateCorrespondent([FromBody] NewCorrespondent correspondent)
    {
        var corr = _mapper.Map<Correspondent>(correspondent);

        corr.Id = 1;
        corr.Slug = "foo";

        return Created($"/api/correspondents/{corr.Id}/", corr);
    }

    [HttpPut("{id:int}", Name = "UpdateCorrespondent")]
    public IActionResult UpdateCorrespondent([FromRoute] int id, [FromBody] Correspondent correspondent)
    {
        return Ok(correspondent);
    }

    [HttpDelete("{id:int}", Name = "DeleteCorrespondent")]
    public IActionResult DeleteCorrespondent([FromRoute] int id)
    {
        return NoContent();
    }
}
