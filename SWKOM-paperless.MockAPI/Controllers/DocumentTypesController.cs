using AutoMapper;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Mock_Server.Models;

namespace Mock_Server.Controllers;

[ApiController]
[Route("/api/document_types/")]
public class DocumentTypesController : ControllerBase
{
    private ILogger<DocumentTypesController> _logger;
    private IMapper _mapper;

    public DocumentTypesController(IMapper mapper, ILogger<DocumentTypesController> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet(Name = "GetDocumentTypes")]
    public IActionResult GetDocumentTypes([FromQuery] GenericFilterModel filter)
    {
        Random r = new Random();
        return Ok(new ListResponse<DocumentType>()
        {
            Count = 6,
            Results = Builder<DocumentType>.CreateListOfSize(6)
                .All().With(p => p.MatchingAlgorithm = r.Next(1, 6))
                .Build()
        });
    }

    [HttpPost(Name = "CreateDocumentType")]
    public IActionResult CreateDocumentType([FromBody] NewDocumentType documentType)
    {
        var docType = _mapper.Map<DocumentType>(documentType);

        docType.Id = 1;
        docType.Slug = "foo";

        return Created($"/api/document_types/{docType.Id}", docType);
    }

    [HttpPut("{id:int}", Name = "UpdateDocumentType")]
    public IActionResult UpdateDocumentType([FromRoute] int id, [FromBody] DocumentType documentType)
    {
        return Ok(documentType);
    }

    [HttpDelete("{id:int}", Name = "DeleteDocumentType")]
    public IActionResult DeleteDocumentType([FromRoute] int id)
    {
        return NoContent();
    }
}
