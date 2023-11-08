using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL;

public class NewTagRepository
{
    private readonly ApplicationDbContext<NewTag> _context;
    
    public NewTagRepository(ApplicationDbContext<NewTag> context)
    {
        _context = context;
    }
    
    public void AddNewTag(NewTag newTag)
    {
        _context.Elements?.Add(newTag);
        _context.SaveChanges();
    }
    
    public IEnumerable<NewTag>? GetAllNewTags()
    {
        return _context.Elements?.ToList();
    }
    
    public NewTag? GetNewTagByName(string name)
    {
        return _context.Elements?.Find(name);
    }
    
    public void DeleteNewTag(string name)
    {
        var newTag = _context.Elements?.Find(name);
        if (newTag == null) return;
        
        _context.Elements?.Remove(newTag);
        _context.SaveChanges();
    }
}