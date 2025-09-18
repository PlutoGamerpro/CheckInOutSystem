using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeRegistration.Classes;
using TimeRegistration.Data;
using TimeRegistration.Interfaces;
using TimeRegistration.Models;

namespace TimeRegistration.Repositories
{
    public class AdminRepo : IAdminRepo
    {
        private readonly AppDbContext _ctx;

        public AdminRepo(AppDbContext ctx)
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

        public User? UpdateUser(UserRecordRequest userRecordRequest) // logic implement in other file
        {
            // could be erorr in this file! 
            var user = _ctx.Users.Find(userRecordRequest.Id);
            _ctx.SaveChanges();
            return user; // maybe not here?
           // return _ctx.Users.Update(user)
        }


        /*
        public User? UpdateUser(int id, User user)
        {
            // Just saves changes to the context, no field update logic
            _ctx.SaveChanges();
            return user;
        }
        */
    }
}