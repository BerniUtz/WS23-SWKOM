using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL;

public class NewTagRepository
{
    private readonly ApplicationDbContext _context;
    
    public NewTagRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public void AddNewTag(NewTag newTag)
    {
        _context.NewTags?.Add(newTag);
        _context.SaveChanges();
    }
    
    public IEnumerable<NewTag>? GetAllNewTags()
    {
        return _context.NewTags?.ToList();
    }
    
    public NewTag? GetNewTagByName(string name)
    {
        return _context.NewTags?.Find(name);
    }
    
    public void DeleteNewTag(string name)
    {
        var newTag = _context.NewTags?.Find(name);
        if (newTag == null) return;
        
        _context.NewTags?.Remove(newTag);
        _context.SaveChanges();
    }
}