using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TimeRegistration.Interfaces;
using TimeRegistration.Services;
using TimeRegistration.Data;
using TimeRegistration.Classes; // added

namespace TimeRegistration.Controllers;

// cspell:ignore Telefonnummeret eksisterer ikke systemet checket igen

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IRegistrationRepo _repo;

    private readonly IAdminRepo _adminRepo;
    private readonly IAdminAuthService _auth;
    private readonly IConfiguration _cfg;
    private readonly AppDbContext _ctx; // changed from DbContext

    public AdminController(IRegistrationRepo repo, IAdminRepo adminRepo, IAdminAuthService auth, IConfiguration cfg, AppDbContext ctx)
    {
        _repo = repo;
        _adminRepo = adminRepo;
        _auth = auth;
        _cfg = cfg;
        _ctx = ctx;
    }

    public record AdminLoginRequest(string Phone, string Secret);

    [HttpPost("login")]
    public IActionResult Login(AdminLoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Phone)) return BadRequest("Phone required");
        var user = _ctx.Users.FirstOrDefault(u => u.Phone == req.Phone);
        if (user == null) return Unauthorized();
        var configSecret = _cfg["Admin:Secret"] ?? "";
        var token = _auth.IssueTokenFor(req.Phone, user.IsAdmin, req.Secret, configSecret);
        if (token == null) return Unauthorized();
        return Ok(new { token, user = user.Name });
    }
    [HttpDelete("user/{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _ctx.Users.Find(id);
        if (user == null) return NotFound();

        _adminRepo.DeleteUser(id, user);
        return NoContent();
    }
    
    [HttpPut("user/{id}")]
    public IActionResult UpdateUser(int id, User user)
    {
        var existingUser = _ctx.Users.Find(id);
        if (existingUser == null) return NotFound();

        // Atualize apenas os campos permitidos
        existingUser.Name = user.Name;
        existingUser.Phone = user.Phone;
        existingUser.IsAdmin = user.IsAdmin;
        // Adicione outros campos editáveis conforme necessário

        _adminRepo.UpdateUser(id, existingUser); // Apenas salva as alterações
        return NoContent();
    }


    

}
