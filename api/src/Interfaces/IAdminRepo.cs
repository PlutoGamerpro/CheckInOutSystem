using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;

namespace TimeRegistration.Interfaces 
{
    public interface IAdminRepo
    {
        User? DeleteUser(int id, User user);

        User? UpdateUser(int id, User user);
    }
}