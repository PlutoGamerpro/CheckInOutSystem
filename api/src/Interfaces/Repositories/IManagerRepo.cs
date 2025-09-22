using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using TimeRegistration.Models;

namespace TimeRegistration.Interfaces
{
    public interface IManagerRepo
    {
        // add extra functions to manager if needded 
        List<User> GetAllManagers(); // get all managers!
        User? UpdateUser(UserRecordRequest userRecordRequest);
    }
}