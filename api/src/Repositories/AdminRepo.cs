using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeRegistration.Classes;
using TimeRegistration.Contracts.Requests;
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

        public void CreateManager(User user)
        {
            _ctx.Users.Add(user);
            _ctx.SaveChanges();
        }

        public User? DeleteManager(DeleteAdminRequest record)
        {
            var user = _ctx.Users.Find(record.user.Id);
            if (user != null)
            {
                _ctx.Users.Remove(user);
                _ctx.SaveChanges();
                return user;
            }
            return null;
        }

        public List<User> GetAllAdmins()
        {
            return _ctx.Users
                .Where(u => u.IsAdmin)
                .AsNoTracking()
                .ToList();
        }

        public User? UpdateUser(UserRecordRequest userRecordRequest)
        {
            if (userRecordRequest == null) return null;

            // Localiza usuário
            var user = _ctx.Users.FirstOrDefault(u => u.Id == userRecordRequest.Id);
            if (user == null) return null;

            // Atualizações parciais (somente se valor fornecido)
            if (!string.IsNullOrWhiteSpace(userRecordRequest.Name))
                user.Name = userRecordRequest.Name.Trim();

            if (!string.IsNullOrWhiteSpace(userRecordRequest.Phone))
                user.Phone = userRecordRequest.Phone.Trim();

            // Campos booleanos opcionais (assumindo nullable bool no DTO)
            // If IsAdmin is not nullable, just assign directly

            // used to be manager 
            user.IsAdmin = userRecordRequest.IsManager;

        
            // Caso exista um campo de senha no DTO (ex: PlainPassword), trate aqui (hash recomendado)
            // if (!string.IsNullOrWhiteSpace(userRecordRequest.PlainPassword)) { ... }

            _ctx.SaveChanges();
            return user;
        }
        /*
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
*/

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