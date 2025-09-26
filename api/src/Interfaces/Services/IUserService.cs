using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using System.Text.RegularExpressions;
using TimeRegistration.Interfaces;
using TimeRegistration.Repositories;
using TimeRegistration.Contracts.Requests;
using TimeRegistration.Data;
namespace TimeRegistration.Services 
{      
  
    public interface IUserService
    { 
       IEnumerable<User> GetAllUsers();
       void CreateUser(CreateUserRequest dto);
       void GetByPhone(string phone);
       void Login(string tlf, UserLoginRequest req);
    }
}