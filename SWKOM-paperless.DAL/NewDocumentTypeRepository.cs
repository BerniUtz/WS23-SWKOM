using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL;

public class NewDocumentTypesRepository
{
    private readonly ApplicationDbContext _context;
    
    public NewDocumentTypesRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public void AddNewDocumentTypes(NewDocumentType NewDocumentTypes)
    {
        _context.NewDocumentTypes?.Add(NewDocumentTypes);
        _context.SaveChanges();
    }
    
    public IEnumerable<NewDocumentType>? GetAllNewDocumentTypess()
    {
        return _context.NewDocumentTypes?.ToList();
    }
    
    public NewDocumentType? GetNewDocumentTypesByName(string name)
    {
        return _context.NewDocumentTypes?.Find(name);
    }
    
    public void DeleteNewDocumentTypes(string name)
    {
        var NewDocumentTypes = _context.NewDocumentTypes?.Find(name);
        if (NewDocumentTypes == null) return;
        
        _context.NewDocumentTypes?.Remove(NewDocumentTypes);
        _context.SaveChanges();
    }
}