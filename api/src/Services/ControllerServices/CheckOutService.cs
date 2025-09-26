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
using TimeRegistration.Models;
using TimeRegistration.Contracts.Requests;
using TimeRegistration.Contracts.Results;
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
            var user = _userRepo.GetAll().FirstOrDefault(u => u.Phone == tlf); 
          
            // Find open registration (registration with CheckIn -> User)
            var openReg = _registrationRepo.GetAll()
                .Where(r => r.FkCheckOutId == null)
                .Join(
                    _checkInRepo.GetAll(),
                    r => r.FkCheckInId, // use check-in id for the post
                    ci => ci.Id, // compare with check-in id
                    (r, ci) => new { r, ci } // Create a object with registration and check-in 
                )
                .Where(x => x.ci.FkUserId == user.Id) // use currently user
                .OrderByDescending(x => x.ci.TimeStart) // takes the latest check-in
                .FirstOrDefault()?.r; // Get post (or null)
               

            if (openReg == null)
                throw new InvalidOperationException("Ingen åben check-in fundet for bruger.");

            var checkOut = new CheckOut
            {
                TimeEnd = DateTime.UtcNow,
                FkUserId = user.Id
            }; 
            _repo.Create(checkOut);            

            // updates or opens registration to date with new checkout             
            openReg.FkCheckOutId = checkOut.Id;
            // _registrationRepo.Update( /*openReg.Id, openReg*/);
            _registrationRepo.Update(new UpdateRegistrationRequest(openReg));

            return new CheckOutResult(checkOut.Id, user.Name, user.Phone);          
        }
        
        public void DeleteCheckOut(int id)
        {
            var checkout = _ctx.CheckOuts.Find(id);
            if (checkout == null)
            {
                throw new Exception("CheckOut not found");
            }
            _repo.Delete(id);
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
        // error this not used any where  
        public CheckOut UpdateCheckOut(int id, CheckOut checkOut)
        {
            throw new Exception("not implement");
        }
    }
}