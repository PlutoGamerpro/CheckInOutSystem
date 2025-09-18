using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Interfaces;
using TimeRegistration.Classes;
using System.Text.RegularExpressions;
using BCrypt.Net;
using TimeRegistration.Data;
using TimeRegistration.Services;

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

        [HttpGet("byphone/{phone}")]
        public IActionResult GetByPhone(string phone)
        {
            try
            {
                _userService.GetByPhone(phone);
                return NoContent(); // olds return ok 
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateUserRequest dto)
        {
            try
            {
               _userService.CreateUser(dto);
                var safeoption = new
                {
                    Name = dto.Name,
                    Phone = dto.Phone,
                   
                };
                return CreatedAtAction(nameof(GetByPhone), new { phone = dto.Phone }, safeoption);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("login-by-phone/{tlf}")]
        public IActionResult LoginByPhone(string tlf, [FromBody] LoginRequest? req)
        {
            try
            {
                _userService.Login(tlf, req!);
                return NoContent(); // old returns ok 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



    }
}

