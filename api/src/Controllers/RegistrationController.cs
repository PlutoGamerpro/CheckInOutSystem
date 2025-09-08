using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Classes;
using TimeRegistration.Interfaces;
using TimeRegistration.Data;
using System.Reflection;
using TimeRegistration.Services;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using TimeRegistration.Filters; 



namespace TimeRegistration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Registration>> GetAll()
        {
            try
            {
                var registrations = _registrationService.GetAllRegistrations();
                return Ok(registrations);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Registration> Get(int id)
        {
            try
            {
                _registrationService.GetRegistrationById(id);
                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public ActionResult<Registration> Create(Registration registration)
        {
            try
            {
                _registrationService.CreateRegistration(registration);
                return CreatedAtAction(nameof(Get), new { id = registration.Id }, registration);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Registration> Update(int id, Registration registration)
        {
            try
            {
                _registrationService.UpdateRegistration(id, registration);
                return AcceptedAtAction(nameof(Get), new { id = registration.Id }, registration);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [AdminAuthorize]
        public ActionResult<Registration> Delete(int id)
        {
            try
            {
                _registrationService.DeleteRegistration(id);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet("admin")]
        [AdminAuthorize]
        public ActionResult<IEnumerable<object>> GetAllAdmin()
        {
            try
            {
                var list = _registrationService.GetAllAdmin();
                return Ok(list);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet("open")]
        [AdminAuthorize]
        public ActionResult<IEnumerable<object>> GetOpen()
        {
            try
            {
                var openRegistrations = _registrationService.GetOpenRegistrations();
                return Ok(openRegistrations);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPatch("{id:int}/checkout")]
        [AdminAuthorize]
        public ActionResult<object> ForceCheckout(int id, [FromBody] ForceCheckoutRequest body)
        {
            try
            {
                _registrationService.ForceCheckout(id, body);
                return NoContent();
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
