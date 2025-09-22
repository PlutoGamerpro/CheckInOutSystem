using System;
using System.Collections.Generic;
using System.Linq;
using TimeRegistration.Interfaces;
using TimeRegistration.Classes;
using TimeRegistration.Data;

namespace TimeRegistration.Services
{
    public class CheckInService : ICheckInService
    {
        private readonly ICheckInRepo _repo;
        private readonly IUserRepo _userRepo;
        private readonly IRegistrationRepo _registrationRepo; 
        private readonly AppDbContext _ctx; 

        public CheckInService(ICheckInRepo repo, IUserRepo userRepo, IRegistrationRepo registrationRepo, AppDbContext ctx)
        {
            _repo = repo;
            _userRepo = userRepo;
            _registrationRepo = registrationRepo;
            _ctx = ctx;
        }

        public CheckInResult CreateCheckInByPhone(string tlf)
        {
            var phone = TimeRegistration.Services.AuthService.NormalizePhone(tlf);
            var user = _userRepo.GetAll().FirstOrDefault(u => (u.Phone ?? "") == phone);
            var name = user?.Name;

            if (user == null)
            {
                throw new KeyNotFoundException("Telefonnummeret eksisterer ikke i systemet");
            }

            var lastCheckIn = _repo.GetAll()
                .Where(c => c.FkUserId == user.Id)
                .OrderByDescending(c => c.TimeStart)
                .FirstOrDefault();
            
            // If there is a CheckIn whose associated registration is open (FkCheckOutId null) -> conflict
            if (lastCheckIn != null)
            {
                var hasOpen = _registrationRepo.GetAll()
                    .Any(r => r.FkCheckInId == lastCheckIn.Id && r.FkCheckOutId == null);
                if (hasOpen)
                    throw new InvalidOperationException("Du er allerede checket ind! Check ud før du kan checke ind igen.");
            }
         
            var checkIn = new CheckIn
            {
                TimeStart = DateTime.UtcNow,
                FkUserId = user.Id
            };
            _repo.Create(checkIn);

            // creates a new open registration (FkCheckOutId null)
            var registration = new Registration
            {
                FkCheckInId = checkIn.Id,
                FkCheckOutId = null,
                FkUserId = user.Id
            };
            _registrationRepo.Create(registration); 
           
            // return results with useful data for controller/frontend
            return new CheckInResult(checkIn.Id, name, phone);
        }

        public void DeleteCheckIn(int id)
        {
            var checkin = _ctx.CheckIns.Find(id);
            if (checkin == null)
            {
                throw new Exception("CheckIn not found");
            }
            _repo.Delete(id);
        }
    

        public IEnumerable<CheckIn> GetAllCheckIns()
        {
            var checkIns = _repo.GetAll().ToList();
            if (checkIns == null || !checkIns.Any())
            {
                throw new Exception("No CheckIns found");
            }
            return checkIns;
        }

        public CheckIn GetCheckInById(int id)
        {
            CheckIn? checkIn = _repo.Get(id);
            if (checkIn == null)
            {
                throw new KeyNotFoundException($"CheckIn with id {id} not found.");
            }
            return checkIn;           
        }

        public CheckIn? UpdateCheckIn(int id, CheckIn checkIn)
        {
            var existing = _repo.Update(id, checkIn);

            if (existing == null)
                throw new KeyNotFoundException($"CheckIn with id {id} not found.");

            return existing;
        }

        public bool GetCheckInStatus(string tlf)
        {
            var phone = AuthService.NormalizePhone(tlf);
            var user = _userRepo.GetAll().FirstOrDefault(u => u.Phone == phone);
            if (user == null)
                throw new KeyNotFoundException("Telefonnummeret eksisterer ikke i systemet");

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
            return isCheckedIn;
        }

        
    }
}
