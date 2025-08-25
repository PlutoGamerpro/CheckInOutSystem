using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Interfaces;
using TimeRegistration.Classes;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace TimeRegistration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _repo;
        private readonly IRegistrationRepo _registrationRepo;

        public UserController(IUserRepo repo, IRegistrationRepo registrationRepo)
        {
            _repo = repo;
            _registrationRepo = registrationRepo;
        }

        // Alterado: retornar também o password (plaintext ou hash, conforme armazenado)
        [HttpGet]
        public ActionResult<IEnumerable<object>> GetAll()
        {
            var users = _repo.GetAll()
                .Select(u => new { u.Id, u.Name, u.Phone, u.IsAdmin, password = u.Password })
                .ToList();
            return Ok(users);
        }

        [HttpGet("byphone/{phone}")]
        public IActionResult GetByPhone(string phone)
        {
            var user = _repo.GetAll().FirstOrDefault(u => u.Phone == phone);
            if (user == null)
                return NotFound();
            // Agora também expõe a senha (apenas para seus testes)
            return Ok(new { user.Id, user.Name, phone = user.Phone, user.IsAdmin, password = user.Password });
        }
        

        [HttpPost]
        public IActionResult Create([FromBody] CreateUserRequest dto)
        {
            if (dto == null) return BadRequest("Body required");

            var name = NormalizeName(dto.Name);
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name required");

            var phone = NormalizePhone(dto.Phone);
            if (string.IsNullOrWhiteSpace(phone))
                return BadRequest("Phone required");
            if (!Regex.IsMatch(phone, @"^\d{8}$"))
                return BadRequest("Phone must be 8 digits");

           
            if (_repo.GetAll().Any(u => u.Phone != null && u.Phone == phone))
                return Conflict("Phone number already exists!");


            if (_repo.GetAll()
                .Where(u => !string.IsNullOrWhiteSpace(u.Name))
                .Any(u => NormalizeName(u.Name) == name))
                return Conflict("Name already exists!");

            var user = new User
            {
                Name = name,
                Phone = phone,
                IsAdmin = dto.IsAdmin ?? false
            };

            // Se password foi fornecido, armazena como veio (plaintext) para testes.
            // Atenção: armazenar senhas em plaintext é inseguro. Use apenas para debug/local.
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.Password = dto.Password;
            }

            // Se for admin, obrigar password
            if (user.IsAdmin)
            {
                if (string.IsNullOrWhiteSpace(dto.Password))
                    return BadRequest("Password required for admin users");
                // já atribuída acima (não hasheada para este teste)
            }

            _repo.Create(user);
            return CreatedAtAction(
                nameof(GetByPhone),
                new { phone = user.Phone },
                new { user.Id, user.Name, phone = user.Phone, user.IsAdmin, password = user.Password });
        }

        // DTO para login via telefone
        public record LoginRequest(string? Password);

        public record CreateUserRequest(string? Name, string? Phone, bool? IsAdmin, string? Password);

        private static string? NormalizePhone(string? v)
        {
            if (string.IsNullOrWhiteSpace(v)) return null;
            var digits = new string(v.Where(char.IsDigit).ToArray());
            return digits;
        }

        private static string NormalizeName(string? v)
        {
            if (string.IsNullOrWhiteSpace(v)) return "";
            var trimmed = string.Join(' ', v.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
            return trimmed;
        }

        [HttpPost("login-by-phone/{tlf}")]
        public IActionResult LoginByPhone(string tlf, [FromBody] LoginRequest? dto)
        {
            // Normaliser telefonnummeret før søgning
            var phone = NormalizePhone(tlf);
            if (string.IsNullOrWhiteSpace(phone))
                return BadRequest("Ugyldigt telefonnummer");

            var user = _repo.GetAll().FirstOrDefault(u => u.Phone == phone);
            if (user == null)
                return NotFound("Telefonnummeret eksisterer ikke i systemet");

            // Hvis brugeren har password sat (eks. admin), kræv og verificer det
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Password))
                    return Unauthorized("Password påkrævet");
                var stored = user.Password;
                // Se parece um hash bcrypt (ex: $2a$... ou $2b$...), use Verify, senão compare plaintext
                if (stored.StartsWith("$2"))
                {
                    if (!BCrypt.Net.BCrypt.Verify(dto.Password, stored))
                        return Unauthorized("Ugyldigt password");
                }
                else
                {
                    if (dto.Password != stored)
                        return Unauthorized("Ugyldigt password");
                }
            }

            // Tjek om brugeren har en åben registration (dvs. FkCheckOutId er 0 eller null)
            var openRegistration = _registrationRepo.GetAll()
                .FirstOrDefault(r => r.FkCheckInId == user.Id && (r.FkCheckOutId == 0 || r.FkCheckOutId == null));

            if (openRegistration != null)
                return Conflict("Du er allerede checket ind! Check ud før du kan checke ind igen.");

            var registration = new Registration
            {
                FkCheckInId = user.Id,
                FkCheckOutId = 0 // eller null hvis nullable
            };
            _registrationRepo.Create(registration);

            return Ok(new { name = user.Name });
        }
    }
}

// cspell:ignore byphone Telefonnummeret eksisterer ikke systemet Tjek brugeren åben allerede checket checke igen hvis


// cspell:ignore byphone Telefonnummeret eksisterer ikke systemet Tjek brugeren åben allerede checket checke igen hvis
