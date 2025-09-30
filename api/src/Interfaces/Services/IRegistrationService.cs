using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using TimeRegistration.Classes;
using TimeRegistration.Contracts.Requests;


namespace TimeRegistration.Services
{  


    public interface IRegistrationService
    {
        IEnumerable<object> GetAllRegistrations();
        IEnumerable<object> GetAllAdmin();
        void GetRegistrationById(int id);
        void CreateRegistration(Registration registration);
        
        // void UpdateRegistration(int id, Registration registration);
        void UpdateRegistration(UpdateRegistrationRequest record);
        void DeleteRegistration(int id); 
        IEnumerable<object> GetOpenRegistrations();         
       void ForceCheckout(int id, ForceCheckoutRequest forceCheckout); 
    }
}