using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Interfaces;
using TimeRegistration.Classes;
using System.Text.RegularExpressions;

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

        [HttpGet]
        public ActionResult<List<User>> GetAll()
        {
            return Ok(_repo.GetAll());
        }

        [HttpGet("byphone/{phone}")]
        public IActionResult GetByPhone(string phone)
        {
            var user = _repo.GetAll().FirstOrDefault(u => u.Phone == phone);
            if (user == null)
                return NotFound();
            return Ok(user);
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

            _repo.Create(user);
            return CreatedAtAction(nameof(GetByPhone), new { phone = user.Phone }, new { user.Id, user.Name, phone = user.Phone, user.IsAdmin });
        }

        public record CreateUserRequest(string? Name, string? Phone, bool? IsAdmin);

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
        public IActionResult LoginByPhone(string tlf)
        {
            var user = _repo.GetAll().FirstOrDefault(u => u.Phone == tlf);
            if (user == null)
                return NotFound("Telefonnummeret eksisterer ikke i systemet");

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
