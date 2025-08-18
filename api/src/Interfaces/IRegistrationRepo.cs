using TimeRegistration.Classes;

namespace TimeRegistration.Interfaces;
public interface IRegistrationRepo
{
    IEnumerable<Registration> GetAll();
    Registration? Get(int id);
    Registration Create(Registration registration);
    Registration? Update(int id, Registration registration);
    Registration? Delete(int id);

    // Admin support:
    // Open = registration without a checkout linkage
    IEnumerable<Registration> GetOpen();
    // Set checkout (creates a checkout record if model supports it, or sets FK directly)
    Registration? SetCheckout(int id, DateTime when);
}
