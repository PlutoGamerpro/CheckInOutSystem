using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using TimeRegistration.Data;
using TimeRegistration.Interfaces;
using TimeRegistration.Models;
using TimeRegistration.Services;


namespace TimeRegistration.Repositories
{
    public class OwnerRepo : IOwnerRepo
    {
        private readonly AppDbContext _ctx;

        public OwnerRepo(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        public List<User> GetAllAdmins()
        {
            return _ctx.Users.Where(u => u.IsAdmin).ToList();
        }
        public User? DeleteAdmin(DeleteAdminRecord record /*int id, User user*/)
        {
            if (record.user != null)
            {
                var userFromDb = _ctx.Users.Find(record.user.Id);
                if (userFromDb != null)
                {
                    _ctx.Users.Remove(userFromDb);
                    _ctx.SaveChanges();
                    return userFromDb;
                }
            }
            return null;
        }

        public void CreateAdmin(User user)
        {
            _ctx.Users.Add(user);
            _ctx.SaveChanges();
        }

        public void RemoteAdmin(int id)
        {

        }
    }
}






/*

public User? DeleteAdmin()
{
if (user != null || user.Id == id)
{
_ctx.Users.Remove(user);
_ctx.SaveChanges();
return user;
}
return null;
}


public void CreateAdmin()
{

}


public User? RemoteAdmin(int id, User user)
{
if (user != null || user.Id == id)
{
_ctx.Users.Remove(user);
_ctx.SaveChanges();
return user;
}
return null;
}
}
*/
