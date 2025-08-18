using TimeRegistration.Classes;
using TimeRegistration.Interfaces;
using TimeRegistration.Data;

namespace TimeRegistration.Repositories
{
    public class CheckOutRepo : ICheckOutRepo
    {
        private readonly AppDbContext _context;

        public CheckOutRepo(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<CheckOut> GetAll()
        {
            return _context.CheckOuts.ToList();
        }

        public CheckOut? Get(int id)
        {
            return _context.CheckOuts.Find(id);
        }

        public CheckOut Create(CheckOut checkOut)
        {
            _context.CheckOuts.Add(checkOut);
            _context.SaveChanges();
            return checkOut;
        }

        public CheckOut? Update(int id, CheckOut checkOut)
        {
            var existing = _context.CheckOuts.Find(id);
            if (existing == null) return null;

            existing.TimeEnd = checkOut.TimeEnd;
            existing.FkUserId = checkOut.FkUserId;
            _context.SaveChanges();
            return existing;
        }

        public CheckOut? Delete(int id)
        {
            var checkOut = _context.CheckOuts.Find(id);
            if (checkOut == null) return null;
            _context.CheckOuts.Remove(checkOut);
            _context.SaveChanges();
            return checkOut;
        }
    }
}
