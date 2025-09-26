using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TimeRegistration.Classes;
using TimeRegistration.Contracts.Requests;
using TimeRegistration.Services;

namespace TimeRegistration.Interfaces 
{
    
    public interface IAdminRepo
    {
        List<User> GetAllAdmins(); // maybe add managers too          
        User? DeleteManager(DeleteAdminRequest record);
        void CreateManager(User user);
        User? UpdateUser(UserRecordRequest userRecordRequest);


        // void RemoteManager(int id); // not secure or delete better 
        // should admin be able to create admins 


    }
}