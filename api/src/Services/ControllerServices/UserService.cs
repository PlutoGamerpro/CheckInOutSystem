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
using System.Text.RegularExpressions;



namespace TimeRegistration.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _repo;

        private readonly IRegistrationRepo _registrationRepo;

        private readonly AppDbContext _ctx;

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
                IsAdmin = dto.IsAdmin ?? false
                // if save hash password here other logic wont worik..
            };

            if (user.IsAdmin)
            {
                if (string.IsNullOrWhiteSpace(dto.Password))
                    throw new Exception("Password required for admin users");
            }
            // denne del gør at check in ikke virkere .....




            // !  (ERROR EVEN WITH THIS OFF)
            /*    
            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                string hashedPhonenumber = BCrypt.Net.BCrypt.HashPassword(dto.Phone);
                user.Phone = hashedPhonenumber;
            }
            */

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                user.Password = hashedPassword;
            }


            _repo.Create(user);
            /*
            return CreatedAtAction(
                nameof(GetByPhone),
                new { phone = user.Phone },
                new { user.Id, user.Name, phone = user.Phone, user.IsAdmin, password = user.Password });
                */
        }

        public IEnumerable<User> GetAllUsers()
        {
            var users = _repo.GetAll().ToList();
            return users;
        }

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

            // Agora também expõe a senha (apenas para seus testes)
            // return Ok(new { user.Id, user.Name, phone = user.Phone, user.IsAdmin, password = user.Password });
        }

        public void Login( string tlf, LoginRequest req)
        {
            // Normaliser telefonnummeret før søgning
            var user = _repo.GetAll().FirstOrDefault(u => u.Phone == tlf);
            //  var phone = NormalizePhone(tlf);
            if (string.IsNullOrWhiteSpace(tlf))
                throw new Exception("Ugyldigt telefonnummer");



            /*        
                       // this can't work more because we encypct the numbers
                        if (!BCrypt.Net.BCrypt.Verify(req.Phone, user.Phone))
                        {
                            return Unauthorized("Telefonnummeret eksisterer ikke i systemet");
                        }
            */



            if (user == null)
                throw new Exception("Telefonnummeret eksisterer ikke i systemet");



            // Tjek om brugeren har en åben registration (dvs. FkCheckOutId er 0 eller null)
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


            // return Ok(new { name = user.Name });
        }

       
    }
}