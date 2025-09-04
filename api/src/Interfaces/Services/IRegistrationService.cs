using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using TimeRegistration.Classes;

namespace TimeRegistration.Services
{

    // with mistake mede this call! 
    /*
        public class ForceCheckout
        {
            public DateTime? When { get; set; }
        }
        */
    
    public record ForceCheckoutRequest(DateTime? When, DateTime? CheckIn);

    public interface IRegistrationService
    {
        IEnumerable<object> GetAllRegistrations();
        IEnumerable<object> GetAllAdmin();
        void GetRegistrationById(int id);
        void CreateRegistration(Registration registration);
        void UpdateRegistration(int id, Registration registration);
        void DeleteRegistration(int id);


        IEnumerable<object> GetOpenRegistrations();

        // ??? maybe error here
       void ForceCheckout(int id, ForceCheckoutRequest forceCheckout);

    }
}