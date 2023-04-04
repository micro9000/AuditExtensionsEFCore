public class UserContext {
  public int UserId {get;set;}
  public string UserName {get;set;}
  public RoleContext Role {get;set;}
  public bool IsSuperUser {get;set;}
  
  public static string Key => nameof(UserContext);
}

public interface ICurrentUserAccessor
{
  UserContext? GetCurrentUser();
}

public class HttpUserContextAccessor : ICurrentUserAccessor
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  
  public HttpUserContextAccessor(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }
  
  public UserContext? GetCurrentUser()
  {
    return _httpContextAccessor.HttpContext?.Items[UserContext.Key] as UserContext;
  }
}

Registration:
services.AddTransient<ICurrentUserAccessor, HttpUserContextAccessor>();
