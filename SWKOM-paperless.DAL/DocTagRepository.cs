using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL;

public class DocTagRepository
{
    private readonly ApplicationDbContext _context;
    
    public DocTagRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public void AddDocTag(DocTag docTag)
    {
        _context.DocTags?.Add(docTag);
        _context.SaveChanges();
    }
    
    public IEnumerable<DocTag>? GetAllDocTags()
    {
        return _context.DocTags?.ToList();
    }
    
    public DocTag? GetDocTagById(int id)
    {
        return _context.DocTags?.Find(id);
    }
    
    public void DeleteDocTag(int id)
    {
        var docTag = _context.DocTags?.Find(id);
        if (docTag == null) return;
        
        _context.DocTags?.Remove(docTag);
        _context.SaveChanges();
    }
}