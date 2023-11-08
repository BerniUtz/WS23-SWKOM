using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL;

public class NewCorrespondentRepository
{
    private readonly ApplicationDbContext<NewCorrespondent> _context;
    
    public NewCorrespondentRepository(ApplicationDbContext<NewCorrespondent> context)
    {
        _context = context;
    }
    
    public void AddNewCorrespondent(NewCorrespondent newCorrespondent)
    {
        _context.Elements?.Add(newCorrespondent);
        _context.SaveChanges();
    }
    
    public IEnumerable<NewCorrespondent>? GetAllNewCorrespondents()
    {
        return _context.Elements?.ToList();
    }
    
    public NewCorrespondent? GetNewCorrespondentById(string name)
    {
        return _context.Elements?.Find(name);
    }
    
    
}