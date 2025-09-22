using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using TimeRegistration.Models;


namespace TimeRegistration.Services
{
    public interface IManagerService
    {
        List<User> GetAllManagers();
        User? UpdateUser(UserRecordRequest userRecordRequest); 
        void DeleteUser(int id);
      

    }
}   
