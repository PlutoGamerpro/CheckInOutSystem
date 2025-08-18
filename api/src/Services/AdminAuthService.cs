using System.Collections.Concurrent;

namespace TimeRegistration.Services;

public interface IAdminAuthService
{
    string? IssueTokenFor(string phone, bool isAdmin, string providedSecret, string configuredSecret);
    bool Validate(string token);
}

public class AdminAuthService : IAdminAuthService
{
    private static readonly ConcurrentDictionary<string, DateTime> _tokens = new();
    private static readonly TimeSpan Lifetime = TimeSpan.FromHours(2);

    public string? IssueTokenFor(string phone, bool isAdmin, string providedSecret, string configuredSecret)
    {
        if (!isAdmin) return null;
        if (string.IsNullOrWhiteSpace(providedSecret) || providedSecret != configuredSecret) return null;
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
}
