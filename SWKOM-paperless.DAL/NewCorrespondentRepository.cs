using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL;

public class NewCorrespondentRepository
{
    private readonly ApplicationDbContext _context;
    
    public NewCorrespondentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public void AddNewCorrespondent(NewCorrespondent newCorrespondent)
    {
        _context.NewCorrespondents?.Add(newCorrespondent);
        _context.SaveChanges();
    }
    
    public IEnumerable<NewCorrespondent>? GetAllNewCorrespondents()
    {
        return _context.NewCorrespondents?.ToList();
    }
    
    public NewCorrespondent? GetNewCorrespondentByName(string name)
    {
        return _context.NewCorrespondents?.Find(name);
    }
    
    public void DeleteNewCorrespondent(string name)
    {
        var newCorrespondent = _context.NewCorrespondents?.Find(name);
        if (newCorrespondent == null) return;
        
        _context.NewCorrespondents?.Remove(newCorrespondent);
        _context.SaveChanges();
    }
}