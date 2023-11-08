using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.DAL;

public class UserInfoRepository
{
    private readonly ApplicationDbContext _context;
    
    public UserInfoRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public void AddUserInfo(UserInfo userInfo)
    {
        _context.UserInfos?.Add(userInfo);
        _context.SaveChanges();
    }
    
    public IEnumerable<UserInfo>? GetAllUserInfos()
    {
        return _context.UserInfos?.ToList();
    }
    
    public UserInfo? GetUserInfoByUsername(string username)
    {
        return _context.UserInfos?.Find(username);
    }
    
    public void DeleteUserInfo(string username)
    {
        var userInfo = _context.UserInfos?.Find(username);
        if (userInfo == null) return;
        
        _context.UserInfos?.Remove(userInfo);
        _context.SaveChanges();
    }
}