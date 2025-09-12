using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using TimeRegistration.Models;


namespace TimeRegistration.Services
{
    public interface IOwnerService
    {
        List<User> GetAllAdmins();
        User? DeleteAdmin(DeleteAdminRecord record);
        // recprd
        
        void CreateAdmin(User user); 
        void RemoteAdmin(int id);       
                      
    }
}   
