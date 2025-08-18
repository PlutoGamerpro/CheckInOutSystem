using TimeRegistration.Classes;

namespace TimeRegistration.Interfaces;

public interface ICheckInRepo
{
    IEnumerable<CheckIn> GetAll();
    CheckIn? Get(int id);
    CheckIn Create(CheckIn checkIn);
    CheckIn? Update(int id, CheckIn checkIn);
    CheckIn? Delete(int id);

}
