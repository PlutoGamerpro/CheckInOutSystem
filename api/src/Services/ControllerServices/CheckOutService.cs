using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Interfaces;
using TimeRegistration.Classes;
using TimeRegistration.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TimeRegistration.Data;






namespace TimeRegistration.Services
{
    public class CheckOutService : ICheckOutService
    {
        private readonly ICheckOutRepo _repo;
        private readonly IUserRepo _userRepo;
        private readonly ICheckInRepo _checkInRepo;
        private readonly IRegistrationRepo _registrationRepo;

        private readonly AppDbContext _ctx;

        public CheckOutService(
            ICheckOutRepo repo,
            IUserRepo userRepo,
            ICheckInRepo checkInRepo,
            AppDbContext ctx,
            IRegistrationRepo registrationRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
            _checkInRepo = checkInRepo;
            _ctx = ctx;
            _registrationRepo = registrationRepo;
        }

        public CheckOutResult CreateCheckOut(string tlf)
        {

            var phone = AuthService.NormalizePhone(tlf);
            // cspell:ignore byphone Telefonnummeret eksisterer ikke systemet seneste checkin brugeren Ingen fundet
            var user = _userRepo.GetAll().FirstOrDefault(u => u.Phone == tlf);

            /*
            if (!BCrypt.Net.BCrypt.Verify(tlf, user.Phone))
            {
                return Unauthorized("Telefonnummeret eksisterer ikke i systemet");
            }
*/
            /*
        if (user == null)
            return NotFound("Telefonnummeret eksisterer ikke i systemet");
*/
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
                throw new InvalidOperationException("Ingen åben check-in fundet for bruger.");

            var checkOut = new CheckOut
            {
                TimeEnd = DateTime.UtcNow,
                FkUserId = user.Id
            };
            _repo.Create(checkOut);

            // Opdater eller åbn registrering til dato med ny checkout

            openReg.FkCheckOutId = checkOut.Id;
            _registrationRepo.Update(openReg.Id, openReg);

            return new CheckOutResult(checkOut.Id, user.Name, user.Phone);
            // return Ok(new { name = user.Name });
        }
        // should not be in the file ,,, the method under...

        public void DeleteCheckOut(int id)
        {
            var checkout = _ctx.CheckOuts.Find(id);
            if (checkout == null)
            {
                throw new Exception("CheckOut not found");
            }
            _repo.Delete(id);

            //return AcceptedAtAction(nameof(Get), new { id = checkOut.Id }, checkOut);
        }

        public IEnumerable<CheckOut> GetAllCheckOuts()
        {
            return _repo.GetAll();
        }

        public CheckOut GetCheckOutById(int id)
        {
            CheckOut? checkOut = _repo.Get(id);
            return checkOut;
        }

        public CheckOut UpdateCheckOut(int id, CheckOut checkOut)
        {
            throw new Exception("not implement");
        }
    }
}