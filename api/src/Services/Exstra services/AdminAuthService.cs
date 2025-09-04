using System.Collections.Concurrent;
using TimeRegistration.Classes;
using TimeRegistration.Interfaces;
namespace TimeRegistration.Services;

public interface IAdminAuthService
{
   // string? IssueTokenFor(string phone, bool isAdmin, string providedSecret, string configuredSecret, string password, string configuredPassword);
    string? IssueTokenFor(string phone, bool isAdmin, string secret, string configSecret, string password);
    bool Validate(string token);
}

public class AdminAuthService : IAdminAuthService
{
    private readonly IUserRepo _repo;
    private static readonly ConcurrentDictionary<string, DateTime> _tokens = new();
    private static readonly TimeSpan Lifetime = TimeSpan.FromHours(2);

    public AdminAuthService(IUserRepo repo)
    {
        _repo = repo;
    }

    public string? IssueTokenFor(string phone, bool isAdmin, string secret, string configSecret, string password)
    {
        if (!isAdmin) return null;

        if (string.IsNullOrWhiteSpace(secret) || secret != configSecret) return null;
        var token = Guid.NewGuid().ToString("N");
        _tokens[token] = DateTime.UtcNow.Add(Lifetime);
        return token;
    }

    public bool Validate(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        if (_tokens.TryGetValue(token, out var exp))
        {
            if (exp > DateTime.UtcNow) return true;
            _tokens.TryRemove(token, out _);
        }
        return false;
    }
       public User? LoginByPhone(string rawPhone, string? password)
    {
        var phone = AuthService.NormalizePhone(rawPhone);
        if (string.IsNullOrWhiteSpace(phone))
            return null;

        var user = _repo.GetAll().FirstOrDefault(u => u.Phone != null && 
            BCrypt.Net.BCrypt.Verify(phone, u.Phone));

        if (user == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            return null;

        return user;
    }

    
}
