using TimeRegistration.Classes;
using TimeRegistration.Interfaces;
using TimeRegistration.Data;

namespace TimeRegistration.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }
        public void Create(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    } 
}
