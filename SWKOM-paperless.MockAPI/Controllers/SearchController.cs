using Microsoft.AspNetCore.Mvc;
using FizzWare.NBuilder;

namespace Mock_Server.Controllers;

[ApiController]
[Route("/api/search/")]
public partial class SearchController : ControllerBase
{
    private ILogger<SearchController> _logger;

    public SearchController(ILogger<SearchController> logger)
    {
        _logger = logger;
    }

    [HttpGet("autocomplete/", Name = "AutoComplete")]
    public IActionResult AutoComplete([FromQuery]string term, [FromQuery]int limit)
    {
        var generator = new RandomGenerator();
        
        return this.Ok( 
            Enumerable.Range(0, 10)
                .Select(el => generator.Phrase(10)
        ));
    }
}
