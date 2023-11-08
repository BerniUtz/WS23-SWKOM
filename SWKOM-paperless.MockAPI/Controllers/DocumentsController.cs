using Microsoft.AspNetCore.Mvc;
using FizzWare.NBuilder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;
using Mock_Server.Models;

namespace Mock_Server.Controllers;

[ApiController]
[Route("/api/documents/")]
public partial class DocumentsController : ControllerBase
{
    private ILogger<DocumentsController> _logger;

    public DocumentsController(ILogger<DocumentsController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetDocuments")]
    public IActionResult GetDocuments([FromQuery] DocumentsFilterModel filter)
    {
        Random r = new Random();

        int count = r.Next(1, 20);

        return this.Ok(new ListResponse<Document>()
        {
            Count = count,
            Next = null,
            Previous = null,
            Results = Builder<Document>.CreateListOfSize(count)
                .All()
                    .With(p => p.ArchiveSerialNumber = null)
                    .With(p => p.ArchivedFileName = p.Id + ".pdf")
                    .With(p => p.OriginalFileName = p.Id + ".pdf")
                    .With(p => p.DocumentType = filter.DocTypeId)
                    .With(p => p.Correspondent = filter.CorrespondentId)
                    .With(p => p.Added = DateTime.Now)
                    .With(p => p.CreatedDate = DateTime.Now)
                    .With(p => p.Created = DateTime.Now)
                    .With(p => p.Modified = DateTime.Now)
                    .With(p => p.Tags = Enumerable.Range(1, r.Next(1, 4)).Select(u => (uint)u).ToArray())
                .Build()
        });
    }

    [HttpGet("{id}", Name = "GetDocument")]
    public IActionResult GetDocument([FromRoute] uint id)
    {
        return this.Ok(new Document()
        {
            Id = id,
            Title = "Document " + id,
            CreatedDate = DateTime.Now,
            DocumentType = 1,
            Added = DateTime.Now,
            Content = "Foo content",
            Tags = new uint[0]
        });
    }

    [HttpGet("{id}/suggestions", Name = "GetDocumentSuggestions")]
    public IActionResult GetDocumentSuggestions([FromRoute] uint id)
    {
        string result = @"{
            ""correspondents"": [
                6
            ],
            ""tags"": [
                3
            ],
            ""document_types"": [
                3
            ],
            ""storage_paths"": [],
            ""dates"": [
                ""2022-06-08"",
                ""2022-12-01"",
                ""2022-12-05""
            ]
        }";

        return this.Ok(JsonSerializer.Deserialize<object>(result));
    }

    [HttpPut("{id:int}", Name = "UpdateDocument")]
    public IActionResult UpdateDocument([FromRoute] int id, [FromBody] Document document)
    {
        return Ok(document);
    }

    [HttpDelete("{id:int}", Name = "DeleteDocument")]
    public IActionResult DeleteDocument([FromRoute] int id)
    {
        return NoContent();
    }

    [HttpPost("post_document", Name = "UploadDocument")]
    public IActionResult UploadDocument([FromForm] string? title,
                                        [FromForm] DateTime? created,
                                        [FromForm(Name = "document_type")] uint? documentType,
                                        [FromForm] uint[] tags,
                                        [FromForm] uint? correspondent,
                                        [FromForm] IEnumerable<IFormFile> document)
    {
        Document doc = new Document()
        {
            Id = 1,
            Title = title,
            CreatedDate = created.GetValueOrDefault(),
            DocumentType = documentType.GetValueOrDefault(),
            Added = DateTime.Now,
            Content = "Foo content"
        };
        return Ok($"/api/documents/{doc.Id}");
    }


    [HttpGet("{id:int}/thumb/", Name = "GetDocumentThumb")]
    public IActionResult GetDocumentThumb([FromRoute] int id)
    {
        Bitmap bitmap = new Bitmap(128, 128, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        Graphics graphics = Graphics.FromImage(bitmap);

        var pen = new Pen(Color.Black, 5);

        graphics.FillRectangle(new SolidBrush(RandomColorGenerator.GetRandomColor()), new Rectangle(0, 0, 128, 128));

        using (var memoryStream = new MemoryStream())
        {
            bitmap.Save(memoryStream, ImageFormat.Jpeg);
            return new FileContentResult(memoryStream.ToArray(), "image/jpeg");
        }
    }

    [HttpGet("{id:int}/download/", Name = "DownloadDocument")]
    public IActionResult DownloadDocument([FromRoute] int id, [FromQuery] bool? original)
    {
        return Ok();
    }

    [HttpGet("{id:int}/preview/", Name = "GetDocumentPreview")]
    public IActionResult GetDocumentPreview([FromRoute] int id)
    {
        Bitmap bitmap = new Bitmap(1000, 5000, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        Graphics graphics = Graphics.FromImage(bitmap);

        var pen = new Pen(Color.Black, 5);

        graphics.FillRectangle(new SolidBrush(RandomColorGenerator.GetRandomColor()), new Rectangle(500, 500, 500, 3000));

        using (var memoryStream = new MemoryStream())
        {
            bitmap.Save(memoryStream, ImageFormat.Png);
            return new FileContentResult(memoryStream.ToArray(), "image/webp");
        }
    }

    [HttpGet("{id:int}/metadata/", Name = "GetDocumentMetadata")]
    public IActionResult GetDocumentMetadata([FromRoute] int id)
    {
        return Ok(Builder<DocumentMetadata>.CreateNew().Build());
    }
}
