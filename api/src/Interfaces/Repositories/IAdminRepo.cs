using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TimeRegistration.Classes;

namespace TimeRegistration.Interfaces 
{
    public interface IAdminRepo
    {
        User? DeleteUser(int id, User user);
        User? UpdateUser(int id, User user);
        IEnumerable<Registration> GetAllToday();
        IEnumerable<Registration> GetAllFromYesterday();
        IEnumerator<Registration> GetAllFromTheWeek();
        IEmailSender<Registration> GetAllFromTheMonth();
        IEmailSender<Registration> GetAllFromTheYear();
    }
}