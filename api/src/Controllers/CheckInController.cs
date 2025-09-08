using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Classes;
using TimeRegistration.Interfaces;
using TimeRegistration.Services;

namespace TimeRegistration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckInController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CheckIn>> GetAll()
        {
            return Ok(_checkInService.GetAllCheckIns());
        }

        [HttpGet("{id}")]
        public ActionResult<CheckIn> Get(int id)
        {

            try
            {
                var checkIn = _checkInService.GetCheckInById(id);
                return Ok(checkIn);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public ActionResult<CheckIn> Create(CheckIn checkIn)
        {
            try
            {
                _checkInService.UpdateCheckIn(checkIn.Id, checkIn); // possibly create a separate Create method in the service
                return CreatedAtAction(nameof(Get), new { id = checkIn.Id }, checkIn);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }


        [HttpPost("byphone/{tlf}")]
        public IActionResult CheckInByPhone(string tlf)
        {
            try
            {
                var result = _checkInService.CreateCheckInByPhone(tlf);
                return Ok(new { name = result.Name, phone = result.Phone, checkInId = result.CheckInId });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + " - " + ex.InnerException?.Message);
            }
        }

        [HttpPut("{id}")]
        public ActionResult<CheckIn> Update(int id, CheckIn checkIn)
        {
            try
            {
                var updated = _checkInService.UpdateCheckIn(id, checkIn);
                return AcceptedAtAction(nameof(Get), new { id = updated.Id }, updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<CheckIn> Delete(int id)
        {
            try
            {
                _checkInService.DeleteCheckIn(id);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        [HttpGet("status/{tlf}")]
        public IActionResult GetCheckInStatus(string tlf)
        {
            try
            {
                bool isCheckedIn = _checkInService.GetCheckInStatus(tlf);
                return Ok(new { phone = tlf, isCheckedIn });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
