using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using TimeRegistration.Models;

namespace TimeRegistration.Interfaces
{
    public interface IOwnerRepo
    {
       List<User> GetAllAdmins();
        //User? DeleteAdmin(int id, User user); 
        User? DeleteAdmin(DeleteAdminRecord record);       
        void CreateAdmin(User user);
        void RemoteAdmin(int id);
    }
}