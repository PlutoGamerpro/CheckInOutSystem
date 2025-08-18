using TimeRegistration.Classes;
using TimeRegistration.Interfaces;
using TimeRegistration.Data;

namespace TimeRegistration.Repositories
{
    public class CheckInRepo : ICheckInRepo
    {
        private readonly AppDbContext _context;

        public CheckInRepo(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<CheckIn> GetAll()
        {
            return _context.CheckIns.ToList();
        }

        public CheckIn? Get(int id)
        {
            return _context.CheckIns.Find(id);
        }

        public CheckIn Create(CheckIn checkIn)
        {
            _context.CheckIns.Add(checkIn);
            _context.SaveChanges(); // Commit to DB
            return checkIn;
        }

        public CheckIn? Update(int id, CheckIn checkIn)
        {
            var existing = _context.CheckIns.Find(id);
            if (existing == null) return null;

            existing.TimeStart = checkIn.TimeStart;
            existing.FkUserId = checkIn.FkUserId;

            _context.SaveChanges();
            return existing;
        }

        public CheckIn? Delete(int id)
        {
            var checkIn = _context.CheckIns.Find(id);
            if (checkIn == null) return null;

            _context.CheckIns.Remove(checkIn);
            _context.SaveChanges();

            return checkIn;
        }
    }
}
