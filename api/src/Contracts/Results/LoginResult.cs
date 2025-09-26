using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeRegistration.Contracts.Results
{
   public record LoginResult(string Token, string UserName);
}