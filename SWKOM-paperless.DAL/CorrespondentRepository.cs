using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL;

public class CorrespondentRepository
{
    private readonly ApplicationDbContext<Correspondent> _context;
    
    public CorrespondentRepository(ApplicationDbContext<Correspondent> context)
    {
        _context = context;
    }
    
    public void AddCorrespondent(Correspondent correspondent)
    {
        _context.Elements?.Add(correspondent);
        _context.SaveChanges();
    }
    
    public IEnumerable<Correspondent>? GetAllCorrespondents()
    {
        return _context.Elements?.ToList();
    }
    
    public Correspondent? GetCorrespondentById(int id)
    {
        return _context.Elements?.Find(id);
    }
    
    public void DeleteCorrespondent(int id)
    {
        var correspondent = _context.Elements?.Find(id);
        if (correspondent == null) return;
        
        _context.Elements?.Remove(correspondent);
        _context.SaveChanges();
    }
}