using System.Collections.Concurrent;
using TimeRegistration.Classes;
using TimeRegistration.Interfaces;
namespace TimeRegistration.Services;


public class AdminAuthService : AuthService
{
      private readonly IUserRepo _repo;
    private readonly ITokenService _tokenService;

    public AdminAuthService(IUserRepo repo, ITokenService tokenService) : base(repo) // bassed added 
    {
        _repo = repo;
        _tokenService = tokenService;
    }
  public string Login(string rawPhone, string? password)
    {
        var phone = NormalizePhone(rawPhone);
        if (string.IsNullOrWhiteSpace(phone))
            throw new Exception("Invalid phone number");

        var user = _repo.GetAll().FirstOrDefault(u => u.Phone == phone);
        if (user == null)
            throw new Exception("User not found");

        // Só exige senha se admin ou manager
        if (user.IsAdmin || user.IsManager)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(user.Password))
                throw new Exception("Password required");
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new Exception("Invalid password");
        }

        return _tokenService.CreateToken(user);
    }
    
}
