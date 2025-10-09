using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;

namespace TimeRegistration.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}