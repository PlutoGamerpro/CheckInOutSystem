using TimeRegistration.Classes;
using TimeRegistration.Models;

namespace TimeRegistration.Interfaces
{
    public record RegistrationResult
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; } // Nullable for open registrations
       
    }

    public interface IRegistrationRepo
    {
        IEnumerable<Registration> GetAll();
        Registration? Get(int id);
        Registration Create(Registration registration);
        //Registration? Update(int id, Registration registration);
        Registration? Update(UpdateRegistrationRecord record); // new apporch
        Registration? Delete(int id);              
        IEnumerable<Registration> GetOpen();

        // Set checkout (creates a checkout record if model supports it, or sets FK directly) 
        Registration? SetCheckout(int id, DateTime when);
    }
}