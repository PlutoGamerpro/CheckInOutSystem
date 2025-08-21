using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeRegistration.Classes;
using TimeRegistration.Data;
using TimeRegistration.Interfaces;

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

        public IEmailSender<Registration> GetAllFromTheMonth()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Registration> GetAllFromTheWeek()
        {
            throw new NotImplementedException();
        }

        public IEmailSender<Registration> GetAllFromTheYear()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Registration> GetAllFromYesterday()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Registration> GetAllToday()
        {
            throw new NotImplementedException();
        }

        public User? UpdateUser(int id, User user)
        {
            // Apenas salva as alterações no contexto, sem lógica de atualização de campos
            _ctx.SaveChanges();
            return user;
        }
    }
}