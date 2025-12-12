using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeRegistration.Contracts.Requests
{    // login to check in / out of system.........
  public record UserLoginRequest(string? Password, string? Phone);

}