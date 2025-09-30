using TimeRegistration.Interfaces;
using TimeRegistration.Classes;
using TimeRegistration.Data;
using System;
using System.Linq;

using TimeRegistration.Contracts.Requests;

namespace TimeRegistration.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepo _repo;
        private readonly AppDbContext _ctx;

     
        public RegistrationService(IRegistrationRepo repo, AppDbContext ctx)
        {
            _repo = repo;
            _ctx = ctx;
        }


        public void CreateRegistration(Registration registration)
        {
            try
            {
                _repo.Create(registration);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating registration: " + ex.Message);
            }         
        }


        public void DeleteRegistration(int id)
        {
            var registration = _repo.Delete(id);
            if (registration == null)
                throw new Exception("NotFound");
        }

        public void ForceCheckout(int id, ForceCheckoutRequest body)
        {
            var when = body?.When ?? DateTime.UtcNow;
            var updated = _repo.SetCheckout(id, when);
            if (updated == null) throw new Exception("NotFound");

            var t = updated.GetType();
            DateTime? checkIn =
                t.GetProperty("CheckIn")?.GetValue(updated) as DateTime? ??
                t.GetProperty("Start")?.GetValue(updated) as DateTime?;           
        }


        public IEnumerable<object> GetAllAdmin()
        {
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

            return list;
        }
             

        public IEnumerable<object> GetAllRegistrations()
        {
            var Registrations = _repo.GetAll().ToList();
            if (Registrations == null || !Registrations.Any())
            {
                throw new Exception("No Registrations found");
            }
            return Registrations;
        }

        public IEnumerable<object> GetOpenRegistrations()
        {
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

            return list;
            
        }

        public void GetRegistrationById(int id)
        {
            Registration? registration = _repo.Get(id);
            if (registration == null)
                throw new Exception("NotFound");             
        }


        public void UpdateRegistration( UpdateRegistrationRequest record/*int id, Registration registration*/)
        {
            var existing = _repo.Update(record);

            if (existing == null)
                throw new Exception("NotFound");           
        }
   }
}

        



