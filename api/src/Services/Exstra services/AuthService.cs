using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Interfaces;
using TimeRegistration.Classes;
using TimeRegistration.Repositories;


namespace TimeRegistration.Services
{
    public class AuthService // make i dont need this file
    {
        private readonly IUserRepo _repo;

        public AuthService(IUserRepo repo)
        {
            _repo = repo;
        }



        public static string NormalizePhone(string phone)
        {
           
            return phone.Trim().Replace(" ", "");
        }
    
        
    }
}