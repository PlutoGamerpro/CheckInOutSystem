using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Interfaces;
using TimeRegistration.Classes;
using TimeRegistration.Contracts.Requests;
using TimeRegistration.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TimeRegistration.Data;
using TimeRegistration.Contracts.Results;
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
        private readonly IConfiguration _cfg;
          private readonly IAdminAuthService _auth;

        public ManagerService(IManagerRepo managerRepo, IExternalRepo externalRepo, AppDbContext ctx, IConfiguration cfg, IAdminAuthService auth)
        {
            _managerRepo = managerRepo;
            _externalRepo = externalRepo;
            _ctx = ctx;
            _cfg = cfg;
            _auth = auth;
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

        public LoginResult? Login(LoginRequest req)
        {
            if (req is null) return null;
            var phone = req.Phone;
            var secret = req.Secret;
            var password = req.Password;

            if (string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(secret) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Phone, secret and password are required");
            }
           
            var user = _ctx.Users.FirstOrDefault(u => u.Phone == phone);
            if (user == null) throw new Exception("Invalid password");

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new Exception("Invalid password");

            var configSecret = _cfg["Admin:Secret"] ?? ""; // does not have to exist 

            var token = _auth.IssueTokenForManager(phone, user.IsManager, secret, configSecret, password);

            // var token = _auth.IssueTokenFor(phone, user.IsAdmin, secret, configSecret, password);
            if (token == null) throw new Exception("Failed to issue token");
            return new LoginResult(token, user.Name);
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
     
