using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Interfaces;
using TimeRegistration.Classes;
using System.Text.RegularExpressions;
using BCrypt.Net;
using TimeRegistration.Data;
using TimeRegistration.Services;


 /*
        private readonly IUserRepo _repo;
        private readonly IRegistrationRepo _registrationRepo;

        private readonly AppDbContext _ctx;
        */

        /*
        public UserController(IUserRepo repo, IRegistrationRepo registrationRepo, AppDbContext ctx)
        {
            /*
            _repo = repo;
            _registrationRepo = registrationRepo;
            _ctx = ctx;
            
        }
    */


namespace TimeRegistration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Alterado: retornar também o password (plaintext ou hash, conforme armazenado)
        [HttpGet]
        public ActionResult<IEnumerable<object>> GetAll()
        {
            try
            {
                var users = _userService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /*
            var users = _repo.GetAll()
                .Select(u => new { u.Id, u.Name, u.Phone, u.IsAdmin, password = u.Password })
                .ToList();
            return Ok(users);
            */


        [HttpGet("byphone/{phone}")]
        public IActionResult GetByPhone(string phone)
        {
            try
            {
                _userService.GetByPhone(phone);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // _userService.GetByPhone(phone);
        /*
        var user = _repo.GetAll().FirstOrDefault(u => u.Phone == phone);
        if (user == null)
            return NotFound();
        // Agora também expõe a senha (apenas para seus testes)
        return Ok(new { user.Id, user.Name, phone = user.Phone, user.IsAdmin, password = user.Password });
        */



        [HttpPost]
        public IActionResult Create([FromBody] CreateUserRequest dto)
        {

            try
            {
                _userService.CreateUser(dto);
                return CreatedAtAction(nameof(GetByPhone), new { phone = dto.Phone }, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /*
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
            // if save hash password here other logic wont worik..
        };

        if (user.IsAdmin)
        {
            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Password required for admin users");
        }
        // denne del gør at check in ikke virkere .....
*/



        // !  (ERROR EVEN WITH THIS OFF) already beeen a comment 
        /*    
        if (!string.IsNullOrWhiteSpace(dto.Phone))
        {
            string hashedPhonenumber = BCrypt.Net.BCrypt.HashPassword(dto.Phone);
            user.Phone = hashedPhonenumber;
        }
        */
        /*
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                user.Password = hashedPassword;
            }


            _repo.Create(user);
            return CreatedAtAction(
                nameof(GetByPhone),
                new { phone = user.Phone },
                new { user.Id, user.Name, phone = user.Phone, user.IsAdmin, password = user.Password });
                */


        // DTO para login via telefone


        /*
        public record LoginRequest(string? Password, string? Phone);


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
        */



        // this should be in the registration(controller),,, because it creates an registration start
        // error this is in the usercontroller .....

        [HttpPost("login-by-phone/{tlf}")]
        public IActionResult LoginByPhone(string tlf, [FromBody] LoginRequest? req)
        {

            try
            {
                _userService.Login(tlf, req!);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /*
    }
      var user = _repo.GetAll().FirstOrDefault(u => u.Phone == tlf);
    //  var phone = NormalizePhone(tlf);
      if (string.IsNullOrWhiteSpace(tlf))
          return BadRequest("Ugyldigt telefonnummer");





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
      */

    }
}

