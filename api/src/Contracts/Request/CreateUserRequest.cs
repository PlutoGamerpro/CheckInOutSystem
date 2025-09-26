using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeRegistration.Contracts.Requests
{
   public record CreateUserRequest(string? Name, string? Phone, bool? IsAdmin, bool IsManager, string? Password);
}