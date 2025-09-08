using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using System.Text.RegularExpressions;
using TimeRegistration.Interfaces;
using TimeRegistration.Repositories;
using TimeRegistration.Data;
namespace TimeRegistration.Services
{
    public record LoginRequest(string? Password, string? Phone);
    public record CreateUserRequest(string? Name, string? Phone, bool? IsAdmin, string? Password);

    public interface IUserService
    { 
       IEnumerable<User> GetAllUsers();
       void CreateUser(CreateUserRequest dto);
       void GetByPhone(string phone);
       void Login(string tlf, LoginRequest req);
    }
}