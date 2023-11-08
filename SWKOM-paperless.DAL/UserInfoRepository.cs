using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL;

public class UserInfoRepository
{
    private readonly ApplicationDbContext<UserInfo> _context;
    
    public UserInfoRepository(ApplicationDbContext<UserInfo> context)
    {
        _context = context;
    }
    
    public void AddUserInfo(UserInfo userInfo)
    {
        _context.Elements?.Add(userInfo);
        _context.SaveChanges();
    }
    
    public IEnumerable<UserInfo>? GetAllUserInfos()
    {
        return _context.Elements?.ToList();
    }
    
    public UserInfo? GetUserInfoById(string username)
    {
        return _context.Elements?.Find(username);
    }
    
    public void DeleteUserInfo(string username)
    {
        var userInfo = _context.Elements?.Find(username);
        if (userInfo == null) return;
        
        _context.Elements?.Remove(userInfo);
        _context.SaveChanges();
    }
}