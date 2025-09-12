using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using TimeRegistration.Models;

namespace TimeRegistration.Services
{  
   // public record ForceCheckoutRequest(DateTime? When, DateTime? CheckIn);

    public interface IRegistrationService
    {
        IEnumerable<object> GetAllRegistrations();
        IEnumerable<object> GetAllAdmin();
        void GetRegistrationById(int id);
        void CreateRegistration(Registration registration);
        
        // void UpdateRegistration(int id, Registration registration);
        void UpdateRegistration(UpdateRegistrationRecord record);
        void DeleteRegistration(int id); 
        IEnumerable<object> GetOpenRegistrations();         
       void ForceCheckout(int id, ForceCheckoutRequest forceCheckout); 
    }
}