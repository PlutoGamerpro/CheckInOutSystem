using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using TimeRegistration.Interfaces;

using TimeRegistration.Data;

namespace TimeRegistration.Repositories
{
    public class ExternalRepo : IExternalRepo
    {
       private readonly AppDbContext _ctx;
   
        public ExternalRepo(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public User? DeleteUser(int id, User user)
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
    }
