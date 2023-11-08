using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL;

public class NewDocumentTypeRepository
{
    private readonly ApplicationDbContext<NewDocumentType> _context;
    
    public NewDocumentTypeRepository(ApplicationDbContext<NewDocumentType> context)
    {
        _context = context;
    }
    
    public void AddNewDocumentType(NewDocumentType newDocumentType)
    {
        _context.Elements?.Add(newDocumentType);
        _context.SaveChanges();
    }
    
    public IEnumerable<NewDocumentType>? GetAllNewDocumentTypes()
    {
        return _context.Elements?.ToList();
    }
    
    public NewDocumentType? GetNewDocumentTypeByName(string name)
    {
        return _context.Elements?.Find(name);
    }
    
    public void DeleteNewDocumentType(string name)
    {
        var newDocumentType = _context.Elements?.Find(name);
        if (newDocumentType == null) return;
        
        _context.Elements?.Remove(newDocumentType);
        _context.SaveChanges();
    }
}