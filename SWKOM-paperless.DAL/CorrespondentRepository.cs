using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL;

public class CorrespondentRepository
{
    private readonly ApplicationDbContext _context;
    
    public CorrespondentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public void AddCorrespondent(Correspondent correspondent)
    {
        _context.Correspondents?.Add(correspondent);
        _context.SaveChanges();
    }
    
    public IEnumerable<Correspondent>? GetAllCorrespondents()
    {
        return _context.Correspondents?.ToList();
    }
    
    public Correspondent? GetCorrespondentById(int id)
    {
        return _context.Correspondents?.Find(id);
    }
    
    public void DeleteCorrespondent(int id)
    {
        var correspondent = _context.Correspondents?.Find(id);
        if (correspondent == null) return;
        
        _context.Correspondents?.Remove(correspondent);
        _context.SaveChanges();
    }
}