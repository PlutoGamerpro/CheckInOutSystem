using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using TimeRegistration.Models;

namespace TimeRegistration.Services
{
    public record AdminLoginResult(string Token, string UserName);
  // not secure if i need this line below
  // public record GetRegistrationsRange (DateTime? StartInclusiveUtc, DateTime? EndExclusiveUtc);
  //  public record AdminLoginResult(string phone, string secret, string password);

  public interface IAdminService
  {
    // AdminLoginResult? Login();

    AdminLoginResult? Login(AdminLoginRequest req);
    void DeleteUser(int id);
    void UpdateUser(int id, User user);

    IEnumerable<object> GetRegistrationsRange(DateTime? startInclusiveUtc, DateTime? endExclusiveUtc);




  // this is a comment becaues i need to return the record and not the normal method 
    // IEnumerable<object> GetRegistrationsRange(DateTime? startInclusiveUtc, DateTime? endExclusiveUtc);
  }
}