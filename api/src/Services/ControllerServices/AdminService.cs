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

using Microsoft.AspNetCore.Http;
using TimeRegistration.Contracts.Results;
using TimeRegistration.Contracts.Requests;
using TimeRegistration.Validation;

namespace TimeRegistration.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRegistrationRepo _repo;
        private readonly IAdminRepo _adminRepo;
        private readonly IExternalRepo _externalRepo;
        //private readonly IAdminAuthService _auth;
        private readonly IConfiguration _cfg;
        private readonly AppDbContext _ctx;

        private readonly ITokenService _tokenService;

        public AdminService(
            IRegistrationRepo repo,
            IAdminRepo adminRepo,
            IExternalRepo externalRepo,
            //  IAdminAuthService auth,
            IConfiguration cfg,
            AppDbContext ctx,
            ITokenService tokenService
            )
        {
            _repo = repo;
            _adminRepo = adminRepo;
            _externalRepo = externalRepo;
            //  _auth = auth;
            _cfg = cfg;
            _ctx = ctx;
            _tokenService = tokenService;

        }


        public LoginResult? Login(LoginRequest req)
        {
            if (req is null) return null;
            var phone = LoginRequestValidator.ValidateOrThrow(req);
            var password = req.Password!.Trim();

            var user = _ctx.Users.FirstOrDefault(u => u.Phone == phone);
            // if (user == null) throw new UnauthorizedAccessException("Invalid password");
            if (user == null || !user.IsManager) throw new UnauthorizedAccessException("Invalid credentials");
            LoginRequestValidator.VerifyPasswordOrThrow(password, user.Password);

            // just to skip error
            // var token = "";
            // return new LoginResult(token, user.Name);
            var token = _tokenService.CreateToken(user);
            return new LoginResult(token, user.Name);

            // var token = _auth.IssueTokenFor(phone, user.IsAdmin, /*secret, */ /*configSecret,*/ password);
            //  if (token == null) throw new InvalidOperationException("Failed to issue token");
            //return new LoginResult(token, user.Name);
        }

        public void DeleteUser(int id)
        {
            var user = _ctx.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            _externalRepo.DeleteUser(id, user);

           // _adminRepo.DeleteUser(id, user);
        }

        public void UpdateUser(/*int id, User user*/ UserRecordRequest userRecordRequest) // if error check old branch for old version 
        {
            var existingUser = _ctx.Users.Find(userRecordRequest.Id);
            if (existingUser == null) throw new KeyNotFoundException("User not found");

            existingUser.Name = userRecordRequest.Name;
            existingUser.Phone = userRecordRequest.Phone;
            existingUser.IsAdmin = userRecordRequest.IsAdmin;

            _adminRepo.UpdateUser(userRecordRequest);
            // this functions is not implemenet in the repo file which mean only frontend update but in backend update does get 
            // changed
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