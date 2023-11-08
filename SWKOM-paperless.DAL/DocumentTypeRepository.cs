using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL;

public class DocumentTypeRepository
{
    private readonly ApplicationDbContext<DocumentType> _context;
    
    public DocumentTypeRepository(ApplicationDbContext<DocumentType> context)
    {
        _context = context;
    }
    
    public void AddDocumentType(DocumentType documentType)
    {
        _context.Elements?.Add(documentType);
        _context.SaveChanges();
    }
    
    public IEnumerable<DocumentType>? GetAllDocumentTypes()
    {
        return _context.Elements?.ToList();
    }
    
    public DocumentType? GetDocumentTypeById(int id)
    {
        return _context.Elements?.Find(id);
    }
    
    public void DeleteDocumentType(int id)
    {
        var documentType = _context.Elements?.Find(id);
        if (documentType == null) return;
        
        _context.Elements?.Remove(documentType);
        _context.SaveChanges();
    }
}