using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Interfaces;
using TimeRegistration.Classes;
using TimeRegistration.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TimeRegistration.Data;
using TimeRegistration.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;


namespace TimeRegistration.Services
{
    public class OwnerService : IOwnerService
    {
       
        private readonly IOwnerRepo _ownerRepo;
     

        public OwnerService(IOwnerRepo ownerRepo)
        {
          
            _ownerRepo = ownerRepo;
           
        }

        public void CreateAdmin(User user)
        {
            try
            {
                _ownerRepo.CreateAdmin(user);
                
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating admin: " + ex.Message);
            }
        }
        public User? DeleteAdmin(DeleteAdminRecord record)
        {
            //   var user = _ctx.Users.Find(id);
            // if (user == null) throw new KeyNotFoundException("User not found");
           
            _ownerRepo.DeleteAdmin(record);            
            return record.user;
            
        }
        public List<User> GetAllAdmins()
        {
            throw new NotImplementedException();
          //  return _ownerService.GetAllAdmins();
        }


        // could implement
        public void RemoteAdmin(int id)
        {
 /// e
        }
        // installere github desktop og setup projektet up 
      
    }
}