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
using TimeRegistration.Contracts.Requests;
using System.Text.RegularExpressions;



namespace TimeRegistration.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _repo;
        private readonly IRegistrationRepo _registrationRepo; 
        private readonly AppDbContext _ctx;
    
        private static readonly Regex PasswordPolicyRegex =
            new(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).+$", RegexOptions.Compiled);

        public UserService(IUserRepo repo, IRegistrationRepo registrationRepo, AppDbContext ctx)
        {
            _repo = repo;
            _registrationRepo = registrationRepo;
            _ctx = ctx;
            
        }

        public void CreateUser(CreateUserRequest dto)
        {
            if (dto == null) throw new Exception("Body required");          

            var name = NormalizeName(dto.Name);
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Name required");

            var phone = NormalizePhone(dto.Phone);
            if (string.IsNullOrWhiteSpace(phone))
                throw new Exception("Phone required");
            if (!Regex.IsMatch(phone, @"^\d{8}$"))
                throw new Exception("Phone must be 8 digits");

            if (_repo.GetAll().Any(u => u.Phone != null && u.Phone == phone))
                throw new Exception("Phone number already exists!");


            if (_repo.GetAll()
                .Where(u => !string.IsNullOrWhiteSpace(u.Name))
                .Any(u => NormalizeName(u.Name) == name))
                throw new Exception("Name already exists!");

            

            var user = new User
            {
                Name = name,
                Phone = phone,
                IsAdmin = dto.IsAdmin ?? false,
                IsManager = dto.IsManager 
            };

            // NOVO: valida e aplica senha (condicional)
            var hashed = EnsurePasswordValid(user, dto.Password);
            user.Password = hashed;

            _repo.Create(user);
         
        }

        public IEnumerable<User> GetAllUsers()
        {
            var users = _repo.GetAll().ToList();
            return users;
        }

        // these two methods are only used internally in the service, if used in other places make them public or add to external service. 
        private static string? NormalizePhone(string? v)
        {
            if (string.IsNullOrWhiteSpace(v)) return null;
            var digits = new string(v.Where(char.IsDigit).ToArray());
            return digits;
        }

        private static string NormalizeName(string? v)
        {
            if (string.IsNullOrWhiteSpace(v)) return "";
            var trimmed = string.Join(' ', v.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
            return trimmed;
        }

        public void GetByPhone(string phone)
        {
            var user = _repo.GetAll().FirstOrDefault(u => u.Phone == phone);
            if (user == null)
                throw new Exception("Telefonnummeret eksisterer ikke i systemet");             
        }

        public void Login( string tlf, UserLoginRequest req)
        {
            
            // normalizePhone number for searching
            var user = _repo.GetAll().FirstOrDefault(u => u.Phone == tlf);
           
            if (string.IsNullOrWhiteSpace(tlf))
                throw new Exception("Ugyldigt telefonnummer"); 

            if (user == null)
                throw new Exception("Telefonnummeret eksisterer ikke i systemet");


            // check if users has a open registration (e.g. FkCheckOutId is 0 or null)
            var openRegistration = _registrationRepo.GetAll()
            .FirstOrDefault(r => r.FkCheckInId == user.Id && (r.FkCheckOutId == 0 || r.FkCheckOutId == null));

            if (openRegistration != null)
                throw new Exception("Du er allerede checket ind! Check ud før du kan checke ind igen.");


            var registration = new Registration
            {
                FkCheckInId = user.Id,
                FkCheckOutId = 0 // eller null hvis nullable
            };
            _registrationRepo.Create(registration);


            
        }

        private string? EnsurePasswordValid(User user, string? plainPassword)
        {
            bool requires = user.IsAdmin || user.IsManager;

            if (requires && string.IsNullOrWhiteSpace(plainPassword))
                throw new Exception("Password required for admin/manager");

            if (string.IsNullOrWhiteSpace(plainPassword))
                return null; // usuário comum sem senha

            if (plainPassword.Length < 8)
                throw new Exception("Password must be at least 8 characters long");

            if (!PasswordPolicyRegex.IsMatch(plainPassword))
                throw new Exception("Password must contain uppercase, lowercase, number and special character");

            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }
    }
}