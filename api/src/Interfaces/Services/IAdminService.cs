using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using TimeRegistration.Models;
using TimeRegistration.Contracts.Requests;
using TimeRegistration.Contracts.Results;

namespace TimeRegistration.Services
{


  public interface IAdminService
  {
    LoginResult? Login(LoginRequest req);
    void UpdateUser(UserRecordRequest userRecordRequest); // new apporch
    IEnumerable<object> GetRegistrationsRange(DateTime? startInclusiveUtc, DateTime? endExclusiveUtc);

    void DeleteUser(int id);

  }
}


//    //void UpdateUser(int id, User user); 