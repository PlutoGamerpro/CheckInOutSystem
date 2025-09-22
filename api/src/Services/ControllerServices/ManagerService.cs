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
    public class ManagerService : IManagerService
    {

        private readonly IManagerRepo _managerRepo; 
        private readonly IExternalRepo _externalRepo;

        private readonly AppDbContext _ctx;

        public ManagerService(IManagerRepo managerRepo, IExternalRepo externalRepo, AppDbContext ctx)
        {
            _managerRepo = managerRepo;
            _externalRepo = externalRepo;
            _ctx = ctx;
        }

        public void DeleteUser(int id)
        {
            var user = _ctx.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            _externalRepo.DeleteUser(id, user);
        }

        public List<User> GetAllManagers()
        {
            _managerRepo.GetAllManagers();
            return _managerRepo.GetAllManagers();
        }

        public User? UpdateUser(UserRecordRequest userRecordRequest) // check admin is not included in the workflow
        {
            var existingUser = _ctx.Users.Find(userRecordRequest.Id);
            if (existingUser == null) throw new KeyNotFoundException("User not found");

            existingUser.Name = userRecordRequest.Name;
            existingUser.Phone = userRecordRequest.Phone;

            _managerRepo.UpdateUser(userRecordRequest);
            return existingUser; 
        }
    }
}
    
/*
                public User? DeleteAdmin(DeleteAdminRecord record)
                {
                    //   var user = _ctx.Users.Find(id);
                    // if (user == null) throw new KeyNotFoundException("User not found");

                    _managerRepo.DeleteAdmin(record);
                    return record.user;

                }
            */
     
