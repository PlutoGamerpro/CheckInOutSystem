using TimeRegistration.Classes;
using TimeRegistration.Interfaces;
using TimeRegistration.Data;

namespace TimeRegistration.Repositories
{
    public class RegistrationRepo : IRegistrationRepo
    {
        private readonly AppDbContext _context;
        public RegistrationRepo(AppDbContext context) => _context = context;

        public IEnumerable<Registration> GetAll() => _context.Registrations.ToList();

        public Registration? Get(int id) => _context.Registrations.Find(id);

        public Registration Create(Registration registration)
        {
            _context.Registrations.Add(registration);
            _context.SaveChanges();
            return registration;
        }

        public Registration? Update(int id, Registration registration)
        {
            var existing = _context.Registrations.Find(id);
            if (existing == null) return null;
            existing.FkCheckInId = registration.FkCheckInId;
            existing.FkCheckOutId = registration.FkCheckOutId;
            _context.SaveChanges();
            return existing;
        }

        public Registration? Delete(int id)
        {
            var existing = _context.Registrations.Find(id);
            if (existing == null) return null;
            _context.Registrations.Remove(existing);
            _context.SaveChanges();
            return existing;
        }

        public IEnumerable<Registration> GetOpen() =>
            _context.Registrations.Where(r => r.FkCheckOutId == null).ToList();

        public Registration? SetCheckout(int id, DateTime when)
        {
            var reg = _context.Registrations.FirstOrDefault(r => r.Id == id);
            if (reg == null) return null;
            if (reg.FkCheckOutId != null) return reg; // allerede lukket

            // Finder CheckIn for at hente brugeren
            var checkIn = _context.CheckIns.FirstOrDefault(ci => ci.Id == reg.FkCheckInId);
            var userId = checkIn?.FkUserId ?? 0;

            var checkOut = new CheckOut
            {
                TimeEnd = when,
                FkUserId = userId
            };
            _context.CheckOuts.Add(checkOut);
            _context.SaveChanges();

            reg.FkCheckOutId = checkOut.Id;
            _context.SaveChanges();
            return reg;
        }
    }
}