using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeRegistration.Services
{
    public interface IAuthService
    {
        string Login(string phone, string? password);
    }
}