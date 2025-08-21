using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Classes;
using TimeRegistration.Interfaces;

namespace TimeRegistration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInRepo _repo;
        private readonly IUserRepo _userRepo;
        private readonly IRegistrationRepo _registrationRepo;

        public CheckInController(ICheckInRepo repo, IUserRepo userRepo, IRegistrationRepo registrationRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
            _registrationRepo = registrationRepo;
        } 

        [HttpGet]
        public ActionResult<IEnumerable<CheckIn>> GetAll()
        {
            return Ok(_repo.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<CheckIn> Get(int id)
        {
            CheckIn? checkIn = _repo.Get(id);
            return checkIn != null ? Ok(checkIn) : NotFound();
        }

        [HttpPost]
        public ActionResult<CheckIn> Create(CheckIn checkIn)
        {
            _repo.Create(checkIn);
            return CreatedAtAction(nameof(Get), new { id = checkIn.Id }, checkIn);
        }


        [HttpPost("byphone/{tlf}")]
        public IActionResult CheckInByPhone(string tlf)
        {
            try
            {
                var user = _userRepo.GetAll().FirstOrDefault(u => u.Phone == tlf);
                if (user == null)
                    return NotFound("Telefonnummeret eksisterer ikke i systemet");

                var lastCheckIn = _repo.GetAll()
                    .Where(c => c.FkUserId == user.Id)
                    .OrderByDescending(c => c.TimeStart)
                    .FirstOrDefault();

                // Hvis der er en CheckIn, hvis tilhørende registrering er åben (FkCheckOutId null) -> konflikt
                if (lastCheckIn != null)
                {
                    var hasOpen = _registrationRepo.GetAll()
                        .Any(r => r.FkCheckInId == lastCheckIn.Id && r.FkCheckOutId == null);
                    if (hasOpen)
                        return Conflict("Du er allerede checket ind! Check ud før du kan checke ind igen.");
                }

                // Opretter en ny CheckIn
                var checkIn = new CheckIn
                {
                    TimeStart = DateTime.UtcNow,
                    FkUserId = user.Id
                };
                _repo.Create(checkIn);

                // Opretter en åben registrering (FkCheckOutId null)
                var registration = new Registration
                {
                    FkCheckInId = checkIn.Id,
                    FkCheckOutId = null,
                    FkUserId = user.Id
                };
                _registrationRepo.Create(registration);

                return Ok(new { name = user.Name, checkInId = checkIn.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + " - " + ex.InnerException?.Message);
            }
        }

        [HttpPut("{id}")]
        public ActionResult<CheckIn> Update(int id, CheckIn checkIn)
        {
            var existing = _repo.Update(id, checkIn);

            if (existing == null)
                return NotFound();

            return AcceptedAtAction(nameof(Get), new { id = existing.Id }, existing);
        }

        [HttpDelete("{id}")]
        public ActionResult<CheckIn> Delete(int id)
        {
            var checkIn = _repo.Delete(id);
            if (checkIn == null)
                return NotFound();

            return AcceptedAtAction(nameof(Get), new { id = checkIn.Id }, checkIn);
        }
        [HttpGet("status/{tlf}")]
        public IActionResult GetCheckInStatus(string tlf)
        {
            var user = _userRepo.GetAll().FirstOrDefault(u => u.Phone == tlf);
            if (user == null)
                return NotFound();

            var lastCheckIn = _repo.GetAll()
                .Where(c => c.FkUserId == user.Id)
                .OrderByDescending(c => c.TimeStart)
                .FirstOrDefault();

            bool isCheckedIn = false;
            if (lastCheckIn != null)
            {
                isCheckedIn = _registrationRepo.GetAll()
                    .Any(r => r.FkCheckInId == lastCheckIn.Id && r.FkCheckOutId == null);
            }

            return Ok(new { isCheckedIn });
        }
            
   }
}
// cspell:ignore Tilføj byphone Telefonnummeret eksisterer ikke systemet seneste brugeren allerede checket igen