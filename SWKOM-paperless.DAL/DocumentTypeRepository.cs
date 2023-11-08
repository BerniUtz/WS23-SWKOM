using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL;

public class DocumentTypeRepository
{
    private readonly ApplicationDbContext _context;
    
    public DocumentTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public void AddDocumentType(DocumentType documentType)
    {
        _context.DocumentTypes?.Add(documentType);
        _context.SaveChanges();
    }
    
    public IEnumerable<DocumentType>? GetAllDocumentTypes()
    {
        return _context.DocumentTypes?.ToList();
    }
    
    public DocumentType? GetDocumentTypeById(int id)
    {
        return _context.DocumentTypes?.Find(id);
    }
    
    public void DeleteDocumentType(int id)
    {
        var documentType = _context.DocumentTypes?.Find(id);
        if (documentType == null) return;
        
        _context.DocumentTypes?.Remove(documentType);
        _context.SaveChanges();
    }
}