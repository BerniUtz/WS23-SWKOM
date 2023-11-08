using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL;

public class DocTagRepository
{
    private readonly ApplicationDbContext<DocTag> _context;
    
    public DocTagRepository(ApplicationDbContext<DocTag> context)
    {
        _context = context;
    }
    
    public void AddDocTag(DocTag docTag)
    {
        _context.Elements?.Add(docTag);
        _context.SaveChanges();
    }
    
    public IEnumerable<DocTag>? GetAllDocTags()
    {
        return _context.Elements?.ToList();
    }
    
    public DocTag? GetDocTagById(int id)
    {
        return _context.Elements?.Find(id);
    }
    
    public void DeleteDocTag(int id)
    {
        var docTag = _context.Elements?.Find(id);
        if (docTag == null) return;
        
        _context.Elements?.Remove(docTag);
        _context.SaveChanges();
    }
}