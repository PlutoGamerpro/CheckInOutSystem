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
    public class ManagerRepo : IManagerRepo
    {
        private readonly AppDbContext _ctx;

        public ManagerRepo(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public List<User> GetAllManagers()
        {
            // i havnt't add the manger role in usrs 
            //  return _ctx.Users.Where(u => u.Role == "Manager").ToList();
          return _ctx.Users.ToList(); 
        }

        public User? UpdateUser(UserRecordRequest userRecordRequest)
        {
            throw new NotImplementedException();
        }

        /*
        public User? DeleteAdmin(DeleteAdminRecord record)
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
        */
        /*
                public void CreateAdmin(User user)
                {
                    _ctx.Users.Add(user);
                    _ctx.SaveChanges();
                }

                public void RemoteAdmin(int id)
                {

                }
                */
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
