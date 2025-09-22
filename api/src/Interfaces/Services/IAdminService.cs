using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using TimeRegistration.Models;

namespace TimeRegistration.Services
{
  public record AdminLoginResult(string Token, string UserName);

  public interface IAdminService
  {
    AdminLoginResult? Login(AdminLoginRequest req);
    void UpdateUser(UserRecordRequest userRecordRequest); // new apporch
    IEnumerable<object> GetRegistrationsRange(DateTime? startInclusiveUtc, DateTime? endExclusiveUtc);

    void DeleteUser(int id);

  }
}


//    //void UpdateUser(int id, User user); 