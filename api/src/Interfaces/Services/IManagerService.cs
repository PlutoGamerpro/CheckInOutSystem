using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using TimeRegistration.Contracts.Requests;
using TimeRegistration.Contracts.Results;

namespace TimeRegistration.Services
{
    
   // public record ManagerLoginResult(string Token, string UserName);
    public interface IManagerService
    {
        List<User> GetAllManagers(); 
        LoginResult? Login(LoginRequest req); 
        User? UpdateUser(UserRecordRequest userRecordRequest);
        void DeleteUser(int id);


    }
}   
