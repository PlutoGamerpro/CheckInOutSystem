using Microsoft.AspNetCore.Mvc;
using TimeRegistration.Services;
using TimeRegistration.Classes;
using TimeRegistration.Contracts.Requests;
using TimeRegistration.Filters;
using TimeRegistration.Data;



namespace TimeRegistration.Controllers
{
    
    [ApiController]
    [Route("api/admin")]
    
    /*
  [Route("api/[controller]")]
    [ApiController]
*/
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminservice;
       // private readonly IManagerService _managerService;
        private readonly AppDbContext _ctx;

        public AdminController(IAdminService adminservice, /*IManagerService managerService*/ AppDbContext ctx)
        {
            _adminservice = adminservice;
           // _managerService = managerService;
            _ctx = ctx;
        }  


   // ...existing code...
[HttpPost("login")]
public IActionResult Login([FromBody] LoginRequest req)
{
    try
    {
        var admin = _adminservice.Login(req);
        if (admin == null)
            return StatusCode(500, "Login failure");
        return Ok(new { token = admin.Token, userName = admin.UserName, role = "admin" });
    }
    catch (KeyNotFoundException)
    {
        return NotFound("User not found");
    }
    catch (UnauthorizedAccessException)
    {
        return Unauthorized("Invalid credentials");
    }
    catch (Exception)
    {
        return StatusCode(500, "Login failure");
    }
    // Hvis du udkommenterer alt nedenfor, skal du stadig returnere noget:
    // return StatusCode(500, "Unexpected error");

// ...existing code...
            /*
            catch (Exception exAdmin)
            {
                // 2ª tentativa: manager
                try
                {
                    
                    var mgr = _managerService.Login(req);
                    return Ok(new { token = mgr.Token, userName = mgr.UserName, role = "manager" });
                    
                }
                
                catch (Exception exMgr)
                {
                    // Decisão de erro combinada
                    /*
                    if (exAdmin is KeyNotFoundException && exMgr is KeyNotFoundException)
                        return NotFound("User not found");
                    if (exAdmin is UnauthorizedAccessException || exMgr is UnauthorizedAccessException)
                        return Unauthorized("Invalid credentials");
                    return StatusCode(500, "Login failure");
                }
            }
            */
        }

        [HttpDelete("user/{id}")]
        [AdminAuthorize]

        public IActionResult DeleteUser(int id)
        {
            try
            {
                _adminservice.DeleteUser(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }

        }


        // [HttpPut("user/{id}")] 
        [HttpPut("user")] // this update users endpoint works on swagger 
        [AdminAuthorize] // could return a token instead change call to take record 

        public IActionResult UpdateUser([FromBody] UserRecordRequest userRecordRequest /* int id, User user*/)
        {
            try
            {
                _adminservice.UpdateUser(userRecordRequest);
                //  _adminservice.UpdateUser(id, user);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }
        }

        private IEnumerable<object> GetRegistrationsJoinRange(DateTime? startInclusiveUtc, DateTime? endExclusiveUtc, DateTime? allowedCheckOutTimeUtc)
        {
            try
            {
                // Use the same joined projection shape as RegistrationService.GetAllAdmin,
                // but optionally restrict by time range (and, if provided, allowedCheckOutTimeUtc).
                var query =
                    from r in _ctx.Registrations
                    join ci in _ctx.CheckIns on r.FkCheckInId equals ci.Id
                    join u in _ctx.Users on ci.FkUserId equals u.Id
                    join co in _ctx.CheckOuts on r.FkCheckOutId equals co.Id into coLeft
                    from co in coLeft.DefaultIfEmpty()
                    select new
                    {
                        Registration = r,
                        CheckIn = ci,
                        User = u,
                        CheckOut = co
                    };

                if (startInclusiveUtc.HasValue)
                {
                    query = query.Where(x => x.CheckIn.TimeStart >= startInclusiveUtc.Value);
                }

                if (endExclusiveUtc.HasValue)
                {
                    query = query.Where(x => x.CheckIn.TimeStart < endExclusiveUtc.Value);
                }

                // If a specific allowed checkout time filter is provided, apply it.
                if (allowedCheckOutTimeUtc.HasValue)
                {
                    query = query.Where(x => x.CheckIn.AllowedCheckOutTime <= allowedCheckOutTimeUtc.Value);
                }

                var list =
                    from x in query
                    orderby x.CheckIn.TimeStart descending
                    select new
                    {
                        id = x.Registration.Id,
                        userId = x.User.Id,
                        userName = x.User.Name,
                        phone = x.User.Phone,
                        countryCode = x.User.CountryCode,
                        checkIn = x.CheckIn.TimeStart,
                        checkOut = x.CheckOut != null ? (DateTime?)x.CheckOut.TimeEnd : null,
                        allowedCheckOutTime = x.CheckIn.AllowedCheckOutTime,
                        checkOutOnTime = x.CheckOut != null
                            ? x.CheckOut.TimeEnd <= x.CheckIn.AllowedCheckOutTime
                            : (bool?)null,
                        timeDiffMinutes = x.CheckOut != null
                            ? (int)((x.CheckOut.TimeEnd - x.CheckIn.AllowedCheckOutTime).TotalMinutes)
                            : (int?)null,
                        isOpen = x.Registration.FkCheckOutId == null,
                        isAdmin = x.User.IsAdmin
                    };

                return list.ToList();
            }
            catch
            {
                return Enumerable.Empty<object>();
            }
        }

        // Possibly create a separate Create method in the service helper for midnight UTC.
        // This avoids Kind=Unspecified and corrects the Npgsql error.
        private static DateTime MidnightUtc(DateTime refUtc)
            => new DateTime(refUtc.Year, refUtc.Month, refUtc.Day, 0, 0, 0, DateTimeKind.Utc);

        private static DateTime ForceUtc(DateTime dt)
            => dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);

        [HttpGet("registrations")]
        public IActionResult GetAllRegistrations()
            => Ok(GetRegistrationsJoinRange(null, null,null));

        [HttpGet("registrations/today")]
        public IActionResult GetTodaysRegistrations()
        {
            var today = MidnightUtc(DateTime.UtcNow);
            return Ok(GetRegistrationsJoinRange(today, today.AddDays(1), null));
        }

        [HttpGet("registrations/yesterday")]
        public IActionResult GetYesterdaysRegistrations()
        {
            var today = MidnightUtc(DateTime.UtcNow);
            var yesterday = today.AddDays(-1);
            return Ok(GetRegistrationsJoinRange(yesterday, today, null));
        }

        [HttpGet("registrations/week")]
        public IActionResult GetWeekRegistrations()
        {
            var today = MidnightUtc(DateTime.UtcNow);
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Sunday=0
            return Ok(GetRegistrationsJoinRange(startOfWeek, startOfWeek.AddDays(7), null));
        }

        [HttpGet("registrations/month")]
        public IActionResult GetMonthRegistrations()
        {
            var today = MidnightUtc(DateTime.UtcNow);
            var startOfMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            return Ok(GetRegistrationsJoinRange(startOfMonth, startOfMonth.AddMonths(1), null));
        }

        [HttpGet("registrations/year")]
        public IActionResult GetYearRegistrations()
        {
            var today = MidnightUtc(DateTime.UtcNow);
            var startOfYear = new DateTime(today.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Ok(GetRegistrationsJoinRange(startOfYear, startOfYear.AddYears(1), null));
        }

        [HttpGet("registrations/by-period")]
        public IActionResult GetByPeriod([FromQuery] string period = "all")
        {
            var today = MidnightUtc(DateTime.UtcNow);
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var startOfYear = new DateTime(today.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return period.ToLower() switch
            {
                "today" => Ok(GetRegistrationsJoinRange(today, today.AddDays(1), null)),
                "yesterday" => Ok(GetRegistrationsJoinRange(today.AddDays(-1), today, null)),
                "week" => Ok(GetRegistrationsJoinRange(startOfWeek, startOfWeek.AddDays(7), null)),
                "month" => Ok(GetRegistrationsJoinRange(startOfMonth, startOfMonth.AddMonths(1), null)),
                "year" => Ok(GetRegistrationsJoinRange(startOfYear, startOfYear.AddYears(1), null)),
                "all" => Ok(GetRegistrationsJoinRange(null, null, null)),
                _ => BadRequest("period inválido")
            };
        }

        [HttpGet("registrations/range")]
        public IActionResult GetRange([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] DateTime? allowedCheckOutTimeUtc = null )
        {
            if (start == default || end == default) return BadRequest("start/end obrigatórios");
            start = ForceUtc(start);
            end = ForceUtc(end);
            
            // Only validate allowedCheckOutTimeUtc if it was explicitly provided
            if (allowedCheckOutTimeUtc.HasValue)
            {
                allowedCheckOutTimeUtc = ForceUtc(allowedCheckOutTimeUtc.Value);
                if (end <= allowedCheckOutTimeUtc) return BadRequest("allowedCheckOutTimeUtc must be less than the end.");
            }
            
            if (end <= start) return BadRequest("end <= start");
            if ((end - start).TotalDays > 400) return BadRequest("Intervalo muito grande (max 400 dias).");
            return Ok(GetRegistrationsJoinRange(start, end, allowedCheckOutTimeUtc));
        }                                               /// after end used to be null 

        // Get full registration history for a single user (by userId)
        [HttpGet("user/{userId}/registrations")]
        [AdminAuthorize]
        public IActionResult GetUserRegistrationHistory(int userId)
        {
            if (userId <= 0) return BadRequest("Invalid user id");

            try
            {
                var query =
                    from r in _ctx.Registrations
                    join ci in _ctx.CheckIns on r.FkCheckInId equals ci.Id
                    join u in _ctx.Users on ci.FkUserId equals u.Id
                    join co in _ctx.CheckOuts on r.FkCheckOutId equals co.Id into coLeft
                    from co in coLeft.DefaultIfEmpty()
                    where u.Id == userId
                    orderby ci.TimeStart descending
                    select new
                    {
                        id = r.Id,
                        userName = u.Name,
                        phone = u.Phone,
                        countryCode = u.CountryCode,
                        checkIn = ci.TimeStart,
                        checkOut = (DateTime?)co.TimeEnd,
                        allowedCheckOutTime = ci.AllowedCheckOutTime,
                        checkOutOnTime = co != null
                            ? co.TimeEnd <= ci.AllowedCheckOutTime
                            : (bool?)null,
                        timeDiffMinutes = co != null
                            ? (int)((co.TimeEnd - ci.AllowedCheckOutTime).TotalMinutes)
                            : (int?)null,
                        isOpen = r.FkCheckOutId == null,
                        isAdmin = u.IsAdmin
                    };

                var list = query.ToList();
                return Ok(list);
            }
            catch
            {
                return StatusCode(500, "Failed to load user registration history");
            }
        }
    }
}
        
        /*        
        [HttpPost("seed-basic")]
        public IActionResult SeedBasic([FromQuery] bool force = false)
        {
            var startOfYear = new DateTime(DateTime.UtcNow.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var endOfYear = startOfYear.AddYears(1);

            var phone = "12345678";
            var user = _ctx.Users.FirstOrDefault(u => u.Phone == phone);
            if (user == null)
            {
                user = new User
                {
                    Name = "Seed User",
                    Phone = phone,
                    IsAdmin = false,
                    IsCheckedIn = false
                };
                _ctx.Users.Add(user);
                _ctx.SaveChanges();
            }

            bool already = _ctx.Registrations
                .Join(_ctx.CheckIns, r => r.FkCheckInId, ci => ci.Id, (r, ci) => new { r, ci })
                .Any(x => x.ci.TimeStart >= startOfYear && x.ci.TimeStart < endOfYear);

            if (already && !force)
                return Ok(new { status = "alreadySeeded", hint = "Use ?force=true para reexecutar." });

            if (force)
            {
                // Limpa somente dados do usuário seed (ordem: Registrations -> CheckOuts -> CheckIns)
                var seedRegs = _ctx.Registrations.Where(r => r.FkUserId == user.Id).ToList();
                if (seedRegs.Count > 0)
                {
                    var checkOutIds = seedRegs.Where(r => r.FkCheckOutId != null).Select(r => r.FkCheckOutId!.Value).ToList();
                    var checkInIds = seedRegs.Select(r => r.FkCheckInId).ToList();

                    _ctx.Registrations.RemoveRange(seedRegs);
                    if (checkOutIds.Any())
                        _ctx.CheckOuts.RemoveRange(_ctx.CheckOuts.Where(co => checkOutIds.Contains(co.Id)));
                    if (checkInIds.Any())
                        _ctx.CheckIns.RemoveRange(_ctx.CheckIns.Where(ci => checkInIds.Contains(ci.Id)));
                    _ctx.SaveChanges();
                }
            }

            DateTime UtcDate(int year, int month, int day, int h, int m) =>
                new DateTime(year, month, day, h, m, 0, DateTimeKind.Utc);

            var today = DateTime.UtcNow;
            var todayMid = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0, DateTimeKind.Utc);

            var sessions = new List<(DateTime start, DateTime end, string label)>
{
    (UtcDate(todayMid.AddMonths(-5).Year, todayMid.AddMonths(-5).Month, 8, 8, 30),
     UtcDate(todayMid.AddMonths(-5).Year, todayMid.AddMonths(-5).Month, 8, 17,  0), "FiveMonthsAgo"),
    (UtcDate(todayMid.AddMonths(-4).Year, todayMid.AddMonths(-4).Month, 9, 8, 45),
     UtcDate(todayMid.AddMonths(-4).Year, todayMid.AddMonths(-4).Month, 9, 17, 15), "FourMonthsAgo"),
    (UtcDate(todayMid.AddMonths(-3).Year, todayMid.AddMonths(-3).Month, 10, 8, 30),
     UtcDate(todayMid.AddMonths(-3).Year, todayMid.AddMonths(-3).Month, 10, 17,  0), "ThreeMonthsAgo"),
    (UtcDate(todayMid.AddMonths(-2).Year, todayMid.AddMonths(-2).Month, 12, 9,  0),
     UtcDate(todayMid.AddMonths(-2).Year, todayMid.AddMonths(-2).Month, 12, 17, 15), "TwoMonthsAgo"),
    (UtcDate(todayMid.AddMonths(-1).Year, todayMid.AddMonths(-1).Month, 14, 8, 45),
     UtcDate(todayMid.AddMonths(-1).Year, todayMid.AddMonths(-1).Month, 14, 17, 10), "OneMonthAgo"),
    (todayMid.AddDays(-14).AddHours(8).AddMinutes(30),
     todayMid.AddDays(-14).AddHours(17), "TwoWeeksAgo"),
    (todayMid.AddDays(-7).AddHours(8).AddMinutes(30),
     todayMid.AddDays(-7).AddHours(17), "WeekAgo"),
    (todayMid.AddDays(-1).AddHours(8).AddMinutes(30),
     todayMid.AddDays(-1).AddHours(17), "Yesterday")
};

            var created = new List<object>();

            foreach (var s in sessions)
            {
                var start = ForceUtc(s.start);
                var end = ForceUtc(s.end);

                bool exists = _ctx.CheckIns.Any(ci =>
                    ci.FkUserId == user.Id &&
                    ci.TimeStart >= start.AddMinutes(-5) &&
                    ci.TimeStart <= start.AddMinutes(5));

                if (exists) continue;

                var ci = new CheckIn { TimeStart = start, FkUserId = user.Id };
                _ctx.CheckIns.Add(ci);
                _ctx.SaveChanges();

                var co = new CheckOut { TimeEnd = end, FkUserId = user.Id };
                _ctx.CheckOuts.Add(co);
                _ctx.SaveChanges();

                var reg = new Registration
                {
                    FkCheckInId = ci.Id,
                    FkCheckOutId = co.Id,
                    FkUserId = user.Id,
                    TimeStart = start
                };
                _ctx.Registrations.Add(reg);
                _ctx.SaveChanges();

                created.Add(new { s.label, regId = reg.Id, checkInId = ci.Id, checkOutId = co.Id, start, end });
            }

            return Ok(new
            {
                status = force ? "seededForced" : "seeded",
                createdCount = created.Count,
                created
            });
        }
    }
}
// ...existing code (resto dos endpoints)...


*/