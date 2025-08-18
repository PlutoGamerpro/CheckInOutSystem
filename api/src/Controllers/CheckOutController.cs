using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Classes;
using TimeRegistration.Interfaces;

namespace TimeRegistration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckOutController : ControllerBase
    {
        private readonly ICheckOutRepo _repo;
        private readonly IUserRepo _userRepo;
        private readonly ICheckInRepo _checkInRepo;
        private readonly IRegistrationRepo _registrationRepo;

        public CheckOutController(
            ICheckOutRepo repo,
            IUserRepo userRepo,
            ICheckInRepo checkInRepo,
            IRegistrationRepo registrationRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
            _checkInRepo = checkInRepo;
            _registrationRepo = registrationRepo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CheckOut>> GetAll()
        {
            return Ok(_repo.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<CheckOut> Get(int id)
        {
            CheckOut? checkOut = _repo.Get(id);
            return checkOut != null ? Ok(checkOut) : NotFound();
        }

        [HttpPost]
        public ActionResult<CheckOut> Create(CheckOut checkOut)
        {
            _repo.Create(checkOut);
            return CreatedAtAction(nameof(Get), new { id = checkOut.Id }, checkOut);
        }

        [HttpPost("byphone/{tlf}")]
        public IActionResult CheckOutByPhone(string tlf)
        {
            // cspell:ignore byphone Telefonnummeret eksisterer ikke systemet seneste checkin brugeren Ingen fundet
            var user = _userRepo.GetAll().FirstOrDefault(u => u.Phone == tlf);
            if (user == null)
                return NotFound("Telefonnummeret eksisterer ikke i systemet");

            // Find åben tilmelding (tilmeld dig CheckIn -> Bruger)
            var openReg = _registrationRepo.GetAll()
                .Where(r => r.FkCheckOutId == null)
                .Join(
                    _checkInRepo.GetAll(),
                    r => r.FkCheckInId, // Brug indtjeknings-id'et for posten
                    ci => ci.Id, // Sammenlign med eller tjek ind
                    (r, ci) => new { r, ci } // Opretter et objekt med registrering og check-in
                )
                .Where(x => x.ci.FkUserId == user.Id) // Kun nuværende bruger
                .OrderByDescending(x => x.ci.TimeStart) // tager det seneste check-in
                .FirstOrDefault()?.r; // Hent posten (eller nullen)

            if (openReg == null)
                return Conflict("Ingen åben check-in fundet for bruger.");

            var checkOut = new CheckOut
            {
                TimeEnd = DateTime.UtcNow,
                FkUserId = user.Id
            };
            _repo.Create(checkOut);

            // Opdater eller åbn registrering til dato med ny checkout
         
            openReg.FkCheckOutId = checkOut.Id;
            _registrationRepo.Update(openReg.Id, openReg);

            return Ok(new { name = user.Name });
        }

        [HttpPut("{id}")]
        public ActionResult<CheckOut> Update(int id, CheckOut checkOut)
        {
            var existing = _repo.Update(id, checkOut);

            if (existing == null)
                return NotFound();

            return AcceptedAtAction(nameof(Get), new { id = existing.Id }, existing);
        }

        [HttpDelete("{id}")]
        public ActionResult<CheckOut> Delete(int id)
        {
            var checkOut = _repo.Delete(id);
            if (checkOut == null)
                return NotFound();

            return AcceptedAtAction(nameof(Get), new { id = checkOut.Id }, checkOut);
        }
    }
}