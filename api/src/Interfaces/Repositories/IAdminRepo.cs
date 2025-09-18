using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TimeRegistration.Classes;
using TimeRegistration.Models;

namespace TimeRegistration.Interfaces 
{
    public interface IAdminRepo
    {
        User? DeleteUser(int id, User user);
        //User? UpdateUser(int id, User user);

        // User? UpdateUser(User user); // new apporch
        User? UpdateUser(UserRecordRequest userRecordRequest);
    }
}