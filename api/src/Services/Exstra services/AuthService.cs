using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Interfaces;
using TimeRegistration.Classes;
using TimeRegistration.Repositories;


namespace TimeRegistration.Services
{
    public class AuthService
    {
        private readonly IUserRepo _repo;

        public AuthService(IUserRepo repo)
        {
            _repo = repo;
        }



        public static string NormalizePhone(string phone)
        {
            // din normaliseringslogik
            return phone.Trim().Replace(" ", "");
        }
    
        
    }
}