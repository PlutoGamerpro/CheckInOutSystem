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

namespace TimeRegistration.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRegistrationRepo _repo;
        private readonly IAdminRepo _adminRepo;
        private readonly IAdminAuthService _auth;
        private readonly IConfiguration _cfg;
        private readonly AppDbContext _ctx;


        public AdminService(
            IRegistrationRepo repo,
            IAdminRepo adminRepo,
            IAdminAuthService auth,
            IConfiguration cfg,
            AppDbContext ctx)
        {
            _repo = repo;
            _adminRepo = adminRepo;
            _auth = auth;
            _cfg = cfg;
            _ctx = ctx;
       
        }

        public AdminLoginResult? Login(AdminLoginRequest req)
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

            var configSecret = _cfg["Admin:Secret"] ?? "";

            var token = _auth.IssueTokenFor(phone, user.IsAdmin, secret, configSecret, password);
            if (token == null) throw new Exception("Failed to issue token");
            return new AdminLoginResult(token, user.Name);
        }

        public void DeleteUser(int id)
        {
            var user = _ctx.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            _adminRepo.DeleteUser(id, user);
        }

        public void UpdateUser(int id, User user)
        {
            var existingUser = _ctx.Users.Find(id);
            if (existingUser == null) throw new KeyNotFoundException("User not found");

            existingUser.Name = user.Name;
            existingUser.Phone = user.Phone;
            existingUser.IsAdmin = user.IsAdmin;

            _adminRepo.UpdateUser(id, existingUser);
        }

       
                                                    
        public IEnumerable<object> GetRegistrationsRange(DateTime? startInclusiveUtc, DateTime? endExclusiveUtc) 
        {        
              
            var query =
                from r in _ctx.Registrations
                join ci in _ctx.CheckIns on r.FkCheckInId equals ci.Id
                join u in _ctx.Users on ci.FkUserId equals u.Id
                join co in _ctx.CheckOuts on r.FkCheckOutId equals co.Id into coLeft
                from co in coLeft.DefaultIfEmpty()
                select new
                {
                    id = r.Id,
                    userName = u.Name,
                    phone = u.Phone,
                    checkIn = ci.TimeStart,
                    checkOut = co != null ? co.TimeEnd : (DateTime?)null,
                    isOpen = r.FkCheckOutId == null
                };

            if (startInclusiveUtc.HasValue)
                query = query.Where(x => x.checkIn >= startInclusiveUtc.Value);
            if (endExclusiveUtc.HasValue)
                query = query.Where(x => x.checkIn < endExclusiveUtc.Value);

            
            return query.OrderByDescending(x => x.checkIn).ToList();
        }      
      
    }
}