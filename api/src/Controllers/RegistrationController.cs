using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Classes;
using TimeRegistration.Interfaces;
using TimeRegistration.Data;
using System.Reflection;
using System.Linq;

namespace TimeRegistration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationRepo _repo;
        private readonly AppDbContext _ctx; // adicionado

        public RegistrationController(IRegistrationRepo repo, AppDbContext ctx)
        {
            _repo = repo;
            _ctx = ctx;
        }

        private bool IsAdminAuthorized()
        {
            var token = Request.Headers["X-Admin-Token"].FirstOrDefault();
            var auth = HttpContext.RequestServices.GetService<TimeRegistration.Services.IAdminAuthService>();
            return auth != null && auth.Validate(token ?? "");
        }

        [HttpGet]
        public ActionResult<IEnumerable<Registration>> GetAll()
        {
            return Ok(_repo.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Registration> Get(int id)
        {
            Registration? registration = _repo.Get(id);
            return registration != null ? Ok(registration) : NotFound();
        }

        [HttpPost]
        public ActionResult<Registration> Create(Registration registration)
        {
            _repo.Create(registration);
            return CreatedAtAction(nameof(Get), new { id = registration.Id }, registration);
        }

        [HttpPut("{id}")]
        public ActionResult<Registration> Update(int id, Registration registration)
        {
            var existing = _repo.Update(id, registration);

            if (existing == null)
                return NotFound();

            return AcceptedAtAction(nameof(Get), new { id = existing.Id }, existing);
        }

        [HttpDelete("{id}")]
        public ActionResult<Registration> Delete(int id)
        {
            if (!IsAdminAuthorized()) return Unauthorized();

            var registration = _repo.Delete(id);
            if (registration == null)
                return NotFound();

            return AcceptedAtAction(nameof(Get), new { id = registration.Id }, registration);
        }
        
        // GET api/registration/admin  (admin listing without compile-time dependency on optional props)
        [HttpGet("admin")]
        public ActionResult<IEnumerable<object>> GetAllAdmin()
        {
            if (!IsAdminAuthorized()) return Unauthorized();

            var list = (from r in _ctx.Registrations
                        join ci in _ctx.CheckIns on r.FkCheckInId equals ci.Id
                        join u in _ctx.Users on ci.FkUserId equals u.Id
                        join co in _ctx.CheckOuts on r.FkCheckOutId equals co.Id into coLeft
                        from co in coLeft.DefaultIfEmpty()
                        select new
                        {
                            id = r.Id,
                            userName = u.Name,
                            phone = u.Phone,
                            checkIn = ci.TimeStart,
                            checkOut = co != null ? co.TimeEnd : (DateTime?)null,
                            isOpen = r.FkCheckOutId == null
                        })
                        .OrderByDescending(x => x.checkIn)
                        .ToList();

            return Ok(list);
        }

        [HttpGet("open")]
        public ActionResult<IEnumerable<object>> GetOpen()
        {
            if (!IsAdminAuthorized()) return Unauthorized();

            var list = (from r in _ctx.Registrations
                        where r.FkCheckOutId == null
                        join ci in _ctx.CheckIns on r.FkCheckInId equals ci.Id
                        join u in _ctx.Users on ci.FkUserId equals u.Id
                        select new
                        {
                            id = r.Id,
                            userName = u.Name,
                            phone = u.Phone,
                            checkIn = ci.TimeStart
                        })
                        .OrderByDescending(x => x.checkIn)
                        .ToList();

            return Ok(list);
        }

        public class ForceCheckoutRequest
        {
            public DateTime? When { get; set; }
        }

        [HttpPatch("{id:int}/checkout")]
        public ActionResult<object> ForceCheckout(int id, [FromBody] ForceCheckoutRequest body)
        {
            if (!IsAdminAuthorized()) return Unauthorized();

            var when = body?.When ?? DateTime.UtcNow;
            var updated = _repo.SetCheckout(id, when);
            if (updated == null) return NotFound();

            var t = updated.GetType();
            DateTime? checkIn =
                t.GetProperty("CheckIn")?.GetValue(updated) as DateTime? ??
                t.GetProperty("Start")?.GetValue(updated) as DateTime?;

            return Ok(new
            {
                id = updated.Id,
                checkIn,
                forcedAt = when,
               
            });
        }
    }
}
